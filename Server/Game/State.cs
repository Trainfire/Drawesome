using System;
using System.Collections.Generic;
using Protocol;

namespace Server.Game
{
    public abstract class State<T> where T : GameData, new()
    {
        public event EventHandler<T> OnEnd;

        public abstract GameState Type { get; }
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

        /// <summary>
        /// Ends this state and informs the GameManager.
        /// </summary>
        /// <param name="fireOnEnd">Should the OnEnd event be fired? If true, this will move the game onto the next state.</param>
        public void EndState(bool fireOnEnd = true)
        {
            Timer.Stop();

            if (fireOnEnd && OnEnd != null)
                OnEnd(this, GameData);
        }

        protected void SetTimer(string name, float duration, bool echoToClients = false)
        {
            Timer = new GameTimer(name, duration);
            Timer.Finish += OnTimerFinish;
            Timer.Tick += Timer_Tick;

            if (echoToClients)
                GameData.Players.ForEach(x => x.AddTimer(Timer.Duration));
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            GameData.Players.ForEach(x => x.SetTimer(Timer.ElapsedTime));
        }

        protected virtual void OnTimerFinish(object sender, EventArgs e)
        {
            EndState();
        }
    }
}
