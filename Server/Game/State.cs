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
        bool EchoToClients { get; set; }

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
        /// <param name="fireOnEnd">Should the OnEnd event be fired? If true, this will invoke the Game's OnEndState.</param>
        public void EndState(bool fireOnEnd = true)
        {
            Timer.Stop();

            if (fireOnEnd && OnEnd != null)
                OnEnd(this, GameData);
        }

        public void SkipState()
        {
            Console.WriteLine("{0}: Skipping...", Type);
            EndState();
        }

        protected void SetTimer(string name, float duration, bool echoToClients = false)
        {
            Timer = new GameTimer(name, duration);
            Timer.Finish += OnTimerFinish;
            Timer.Tick += Timer_Tick;
            EchoToClients = echoToClients;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (EchoToClients)
                GameData.Players.ForEach(x => x.SetTimer(Timer.Duration - Timer.ElapsedTime));
        }

        protected virtual void OnTimerFinish(object sender, EventArgs e)
        {
            EndState();
        }
    }
}
