using System;
using System.Collections.Generic;
using Protocol;

namespace Server.Game
{
    public abstract class Game
    {
        protected Queue<State> States { get; private set; }
        protected List<Player> Players { get; private set; }

        public Game(List<Player> players)
        {
            States = new Queue<State>();
            Players = players;
            Players.ForEach(x => x.OnMessage += OnPlayerMessage);
        }

        public void Start()
        {
            if (States.Count != 0)
                MoveToNextState();
        }

        protected virtual void OnPlayerMessage(object sender, Message e)
        {
            if(States.Count != 0)
                States.Peek().OnMessage(e);
        }

        protected void AddState(State state)
        {
            States.Enqueue(state);
        }

        protected void MoveToNextState()
        {
            var currentState = States.Dequeue();
            currentState.OnEnd -= OnStateEnd;

            var nextState = States.Peek();
            nextState.OnEnd += OnStateEnd;
            nextState.Begin();
        }

        void OnStateEnd(object sender, EventArgs e)
        {
            MoveToNextState();
        }
    }
}
