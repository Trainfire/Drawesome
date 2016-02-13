using System;
using System.Collections.Generic;
using Protocol;

namespace Server.Game
{
    /// <summary>
    /// Each Game has associated GameData. GameData will be passed between different states of the game and updated throughout.
    /// </summary>
    /// <typeparam name="TData">The GameData associated with this Game.</typeparam>
    public abstract class Game<TData> where TData : GameData, new()
    {
        protected TData GameData { get; set; }
        protected State<TData> CurrentState { get; private set; }
        protected Settings Settings { get; private set; }

        protected abstract string Name { get; }

        Dictionary<GameState, State<TData>> States { get; set; }

        public Game(Settings settings)
        {
            Log("Initialised");
            Settings = settings;
            States = new Dictionary<GameState, State<TData>>();
        }

        public virtual void Start(List<Player> players)
        {
            Log("Started");
            GameData = new TData();
            GameData.Settings = Settings;
            GameData.Players = new List<Player>();
            GameData.Players = players;
            GameData.Players.ForEach(x => x.OnMessageString += OnPlayerMessage);
        }

        public virtual void StartNewRound()
        {
            Log("Start New Round");
        }

        public virtual void Restart()
        {
            Log("Restarting game...");
            End();
        }

        public void End()
        {
            Log("Ended");
            if (CurrentState != null)
                CurrentState.EndState(false);
        }        

        protected virtual void OnGameOver()
        {
            Log("Game Over!");
        }

        protected virtual void OnPlayerMessage(object sender, string e)
        {
            var player = sender as Player;

            // Skip current state
            Message.IsType<ClientMessage.Game.SkipPhase>(e, (data) =>
            {
                SkipState();
            });

            if (CurrentState != null)
                CurrentState.OnPlayerMessage(player, e);
        }

        protected void SetState(GameState state, GameData gameData)
        {
            if (CurrentState != null)
                CurrentState.OnEnd -= EndState;

            if (!States.ContainsKey(state))
            {
                // TODO: Flag as fatal error
                Log("State {0} does not exist.", state.ToString());
                return;
            }

            CurrentState = States[state];

            Log("{0}: {1}", "Change state to", state);

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

        protected void Log(string message, params object[] args)
        {
            var str = string.Format(message, args);
            Console.WriteLine("{0}: {1}", Name, str);
        }

        void SkipState()
        {
            if (CurrentState != null)
                CurrentState.EndState();
        }

        void EndState(object sender, TData gameData)
        {
            if (!IsGameOver())
            {
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
