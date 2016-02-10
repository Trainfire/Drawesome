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

        public virtual void OnPlayerMessage(PlayerData player, string json)
        {

        }

        /// <summary>
        /// Ends this state and informs the GameManager.
        /// </summary>
        protected void EndState()
        {
            Timer.Stop();

            if (OnEnd != null)
                OnEnd(this, GameData);
        }

        public void SkipState()
        {
            EndState();
        }

        protected void SetTimer(string name, float duration)
        {
            Timer = new GameTimer(name, duration);
            Timer.Finish += OnTimerFinish;
        }

        protected virtual void OnTimerFinish(object sender, EventArgs e)
        {
            EndState();
        }
    }
}
