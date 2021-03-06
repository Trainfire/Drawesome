using System;
using System.Collections.Generic;
using Protocol;

namespace Server.Game
{
    /// <summary>
    /// Each Game has associated GameData. GameData will be passed between different states of the game and updated throughout.
    /// </summary>
    /// <typeparam name="TData">The GameData associated with this Game.</typeparam>
    public abstract class Game<TData> : IConnectionMessageHandler, ILogger where TData : GameData, new()
    {
        public event EventHandler OnEnd;

        protected TData GameData { get; set; }
        protected Settings Settings { get; private set; }

        public abstract string LogName { get; }

        ConnectionsHandler ConnectionsHandler { get; set; }
        State<TData> CurrentState { get; set; }
        Dictionary<GameState, State<TData>> States { get; set; }
        GameTimer TransitionTimer { get; set; }
        bool GameEnded { get; set; }

        public Game(ConnectionsHandler connectionsHandler, Settings settings)
        {
            Logger.Log(this, "Initialised");

            ConnectionsHandler = connectionsHandler;
            connectionsHandler.AddMessageListener(this);

            Settings = settings;
            States = new Dictionary<GameState, State<TData>>();
        }

        public virtual void Start(List<Player> players)
        {
            Logger.Log(this, "Started");
            GameData = new GameData(players, Settings) as TData;
        }

        public virtual void StartNewRound()
        {
            Logger.Log(this, "Start New Round");
        }

        public virtual void Restart()
        {
            Logger.Log(this, "Restarting game...");
            End();
        }

        public void End()
        {
            Logger.Log(this, "Ended");

            GameEnded = true;

            CancelTransitionTimer();
            ConnectionsHandler.RemoveMessageListener(this);

            if (OnEnd != null)
                OnEnd(this, null);
        }

        public void HandleMessage(Player player, string json)
        {
            // Skip current state
            Message.IsType<ClientMessage.Game.SkipPhase>(json, (data) =>
            {
                if (player.IsAdmin)
                    SkipState();
            });

            if (CurrentState != null)
                CurrentState.HandleMessage(player, json);
        }

        protected void SetState(GameState state, float transitionTime)
        {
            if (CurrentState != null)
                CurrentState.OnEnd -= EndState;

            if (!States.ContainsKey(state))
            {
                Logger.Warn(this, "State {0} does not exist.", state.ToString());
                return;
            }

            CurrentState = States[state];

            // Add transition before moving onto next state
            TransitionTimer = new GameTimer("Transition", transitionTime, () =>
            {
                Logger.Log(this, "{0}: {1}", "Change state to", CurrentState.Type.ToString());

                // Notify clients of state change
                GameData.Players.ForEach(x => x.SendMessage(new ServerMessage.Game.ChangeState(CurrentState.Type)));

                CurrentState.OnEnd += EndState;
                CurrentState.Begin(GameData);
            });

            GameData.Players.ForEach(x => x.SendTransitionPeriod(TransitionTimer.Duration));
        }

        protected TState AddState<TState>(GameState stateType, TState stateInstance) where TState : State<TData>
        {
            if (!States.ContainsKey(stateType) && !States.ContainsValue(stateInstance))
                States.Add(stateType, stateInstance);

            return stateInstance;
        }

        void CancelTransitionTimer()
        {
            if (TransitionTimer != null)
                TransitionTimer.Stop();
        }

        void SkipState()
        {
            if (CurrentState != null)
                CurrentState.EndState(GameStateEndReason.Skipped);
        }

        void EndState(object sender, State<TData>.StateEventArgs args)
        {
            var state = sender as State<TData>;

            if (!GameEnded)
            {
                var log = string.Format("End state '{0}'. (Reason: {1})", state.Type.ToString(), args.EndReason.ToString());
                Logger.Log(this, log);

                GameData.Players.ForEach(x => x.SendMessage(new ServerMessage.Game.EndState(args.EndReason)));
                OnEndState(state.Type);
            }
        }

        protected abstract void OnEndState(GameState endingState);
    }
}
