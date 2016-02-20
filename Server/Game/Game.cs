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
        protected TData GameData { get; set; }
        protected Settings Settings { get; private set; }

        public abstract string LogName { get; }

        State<TData> CurrentState { get; set; }
        Dictionary<GameState, State<TData>> States { get; set; }

        public Game(ConnectionsHandler connectionsHandler, Settings settings)
        {
            Logger.Log(this, "Initialised");
            Settings = settings;
            States = new Dictionary<GameState, State<TData>>();
            connectionsHandler.AddMessageListener(this);
        }

        public virtual void Start(List<Player> players)
        {
            Logger.Log(this, "Started");
            GameData = new TData();
            GameData.Settings = Settings;
            GameData.Players = new List<Player>();
            GameData.Players = players;
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
            if (CurrentState != null)
                CurrentState.EndState(GameStateEndReason.GameEnded, false);
        }        

        protected virtual void OnGameOver()
        {
            Logger.Log(this, "Game Over!");
        }

        public void HandleMessage(Player player, string json)
        {
            // Skip current state
            Message.IsType<ClientMessage.Game.SkipPhase>(json, (data) =>
            {
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
            var transitionTimer = new GameTimer("Transition", transitionTime, () =>
            {
                Logger.Log(this, "{0}: {1}", "Change state to", CurrentState.Type.ToString());

                // Notify clients of state change
                GameData.Players.ForEach(x => x.SendMessage(new ServerMessage.Game.ChangeState(CurrentState.Type)));

                CurrentState.OnEnd += EndState;
                CurrentState.Begin(GameData);
            });

            GameData.Players.ForEach(x => x.SendTransitionPeriod(transitionTimer.Duration));
        }

        protected TState AddState<TState>(GameState stateType, TState stateInstance) where TState : State<TData>
        {
            if (!States.ContainsKey(stateType) && !States.ContainsValue(stateInstance))
                States.Add(stateType, stateInstance);

            return stateInstance;
        }

        void SkipState()
        {
            if (CurrentState != null)
                CurrentState.EndState(GameStateEndReason.Skipped);
        }

        void EndState(object sender, State<TData>.StateEventArgs args)
        {
            var state = sender as State<TData>;

            if (!IsGameOver())
            {
                var log = string.Format("End state '{0}'. (Reason: {1})", state.Type.ToString(), args.EndReason.ToString());
                Logger.Log(this, log);

                GameData.Players.ForEach(x => x.SendMessage(new ServerMessage.Game.EndState(args.EndReason)));
                OnEndState(state.Type);
            }
            else
            {
                OnGameOver();
            }
        }

        protected abstract void OnEndState(GameState endingState);
        protected abstract bool IsGameOver();
    }
}
