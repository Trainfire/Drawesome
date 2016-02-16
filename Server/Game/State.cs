using System;
using System.Collections.Generic;
using Protocol;

namespace Server.Game
{
    public abstract class State<T> where T : GameData, new()
    {
        /// <summary>
        /// This event is used to tell the instance of Game that this state has ended.
        /// </summary>
        public event EventHandler<T> OnEnd;

        public abstract GameState Type { get; } // TODO: Move into interface or different base class
        public T GameData { get; protected set; }

        GameTimer Timer { get; set; }

        public void Begin(T gameData)
        {
            GameData = gameData;
            Timer = new GameTimer();
            OnBegin();
        }

        protected virtual void OnBegin()
        {

        }

        public virtual void OnPlayerMessage(Player player, string json)
        {

        }

        protected virtual void OnCountdownFinish(object sender, EventArgs e)
        {
            EndState();
        }

        /// <summary>
        /// Ends this state and informs the Game.
        /// </summary>
        /// <param name="fireOnEnd">Should the OnEnd event be fired? If true, this will move the game onto the next state.</param>
        public void EndState(bool fireOnEnd = true)
        {
            //Timer.Stop();

            if (fireOnEnd)
                OnEndState();
        }

        /// <summary>
        /// Ends this state and creates a transition timer. The Game will be informed of the state change when the transition timer reaches 0.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="fireOnEnd">Should the OnEnd event be fired? If true, this will move the game onto the next state.</param>
        public void EndState(float duration, GameTransition transition, bool fireOnEnd = true)
        {
            // Stop countdown timer
            Timer.Stop();

            var transitionTimer = new GameTimer("Transition", duration);
            transitionTimer.Finish += (sender, args) =>
            {
                EndState(fireOnEnd);
            };

            GameData.Players.ForEach(x => x.SendTransitionPeriod(duration, transition));
        }

        protected virtual void OnEndState()
        {
            if (OnEnd != null)
                OnEnd(this, GameData);
        }

        /// <summary>
        /// Adds a countdown timer to this state. When the timer finishes, it will end this state.
        /// </summary>
        /// <param name="name">Optional name for this timer.</param>
        /// <param name="duration"></param>
        /// <param name="echoToClients">Inform the client of a timer being added?</param>
        protected void SetCountdownTimer(string name, float duration, bool echoToClients = false)
        {
            Timer = new GameTimer(name, duration);
            Timer.Finish += OnCountdownFinish;
            Timer.Tick += OnTimerTick;

            if (echoToClients)
                GameData.Players.ForEach(x => x.AddTimer(Timer.Duration));
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            GameData.Players.ForEach(x => x.SetTimer(Timer.Duration - Timer.ElapsedTime));
        }
    }
}
