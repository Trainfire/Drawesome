using System;
using System.Collections.Generic;
using Protocol;

namespace Server.Game
{
    /// <summary>
    /// Each Game has associated GameData. GameData will be passed between different states of the game and updated throughout.
    /// </summary>
    /// <typeparam name="TData">The GameData associated with this Game.</typeparam>
    public abstract class Game<TData> where TData : GameData
    {
        protected TData GameData { get; set; }
        protected State<TData> CurrentState { get; private set; }

        Dictionary<GameState, State<TData>> States { get; set; }

        public Game()
        {
            States = new Dictionary<GameState, State<TData>>();
            GameData = default(TData);
            GameData.Players.ForEach(x => x.OnMessageString += OnPlayerMessage);
        }

        protected virtual void OnPlayerMessage(object sender, string e)
        {
            if (CurrentState != null)
                CurrentState.OnMessage(e);
        }

        protected void SetState(GameState state, GameData gameData)
        {
            if (CurrentState != null)
                CurrentState.OnEnd -= EndState;

            CurrentState = States[state];

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

        void EndState(object sender, TData gameData)
        {
            // Pass the latest copy of gamedata from the ending state back into the game manager.
            OnEndState(GameData);
        }

        protected abstract void OnEndState(TData gameData);
    }
}
