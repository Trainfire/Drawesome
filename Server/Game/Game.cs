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
        protected State<TData> CurrentState { get; private set; }
        protected Settings Settings { get; private set; }

        public abstract string LogName { get; }

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
                CurrentState.EndState(false);
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

        protected void SetState(GameState state, GameData gameData)
        {
            if (CurrentState != null)
                CurrentState.OnEnd -= EndState;

            if (!States.ContainsKey(state))
            {
                // TODO: Flag as fatal error
                Logger.Log(this, "State {0} does not exist.", state.ToString());
                return;
            }

            CurrentState = States[state];

            Logger.Log(this, "{0}: {1}", "Change state to", CurrentState.Type.ToString());

            // Notify clients of state change
            GameData.Players.ForEach(x => x.SendMessage(new ServerMessage.Game.StateChange(CurrentState.Type)));

            CurrentState.OnEnd += EndState;
            CurrentState.Begin(GameData);
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
                CurrentState.EndState();
        }

        void EndState(object sender, TData gameData)
        {
            var state = sender as State<TData>;

            if (!IsGameOver())
            {
                Logger.Log(this, "End " + state.Type.ToString());

                // Pass the latest copy of gamedata from the ending state back into the game manager.
                OnEndState(GameData);
            }
            else
            {
                OnGameOver();
            }
        }

        protected abstract void OnEndState(TData gameData);
        protected abstract bool IsGameOver();
    }
}
