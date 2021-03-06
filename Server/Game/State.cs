using System;
using System.Collections.Generic;
using Protocol;

namespace Server.Game
{
    public abstract class State<T> : IConnectionMessageHandler where T : GameData, new()
    {
        /// <summary>
        /// This event is used to tell the instance of Game that this state has ended.
        /// </summary>
        public event EventHandler<StateEventArgs> OnEnd;

        public abstract GameState Type { get; } // TODO: Move into interface or different base class
        public T GameData { get; protected set; }

        protected ResponseHandler<Player> ResponseHandler { get; private set; }
        protected Settings Settings { get; private set; }

        GameTimer Timer { get; set; }

        public class StateEventArgs : EventArgs
        {
            public T GameData { get; protected set; }
            public GameStateEndReason EndReason { get; protected set; }

            public StateEventArgs(T gameData, GameStateEndReason endReason)
            {
                GameData = gameData;
                EndReason = endReason;
            }
        }

        public State(Settings settings)
        {
            Settings = settings;
            ResponseHandler = new ResponseHandler<Player>();
        }

        public void Begin(T gameData)
        {
            GameData = gameData;

            foreach (var player in gameData.Players)
            {
                player.OnConnectionClosed += OnPlayerConnectionClosed;
                player.OnLeaveRoom += OnPlayerLeaveRoom;
            }
            GameData.Players.ForEach(x => x.OnConnectionClosed += OnPlayerConnectionClosed);

            Timer = new GameTimer();
            OnBegin();
        }

        void OnPlayerLeaveRoom(object sender, EventArgs e)
        {
            var player = sender as Player;
            player.OnLeaveRoom -= OnPlayerLeaveRoom;
            UpdateState(player);
        }

        void OnPlayerConnectionClosed(object sender, PlayerConnectionClosed e)
        {
            var player = sender as Player;
            player.OnConnectionClosed -= OnPlayerConnectionClosed;
            UpdateState(player);
        }

        void RemovePlayerListeners(Player player)
        {
            player.OnConnectionClosed -= OnPlayerConnectionClosed;
            player.OnLeaveRoom -= OnPlayerLeaveRoom;
        }

        protected virtual void OnBegin()
        {

        }

        public virtual void HandleMessage(Player player, string json)
        {

        }

        /// <summary>
        /// Ends this state and informs the Game.
        /// </summary>
        /// <param name="fireOnEnd">Should the OnEnd event be fired? If true, this will move the game onto the next state.</param>
        public void EndState(bool fireOnEnd = true)
        {
            EndState(GameStateEndReason.Normal, fireOnEnd);
        }

        public void EndState(GameStateEndReason reason, bool fireOnEnd = true)
        {
            ResponseHandler.Clear();

            if (Timer != null)
                Timer.Stop();

            if (fireOnEnd)
                OnEndState(reason);
        }

        protected virtual void OnEndState(GameStateEndReason reason)
        {
            if (OnEnd != null)
                OnEnd(this, new StateEventArgs(GameData, reason));            
        }

        /// <summary>
        /// Adds a countdown timer to this state. When the timer finishes, it will end this state.
        /// </summary>
        /// <param name="name">Optional name for this timer.</param>
        /// <param name="duration"></param>
        /// <param name="echoToClients">Inform the client of a timer being added?</param>
        protected void SetCountdownTimer(string name, float duration, bool echoToClients = false)
        {
            Timer = new GameTimer(Type.ToString() + " - " + name, duration, () =>
            {
                EndState(GameStateEndReason.TimerExpired);
            });
            Timer.Tick += OnTimerTick;

            if (echoToClients)
                GameData.Players.ForEach(x => x.AddTimer(Timer.Duration));
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            GameData.Players.ForEach(x => x.SetTimer(Timer.Duration - Timer.ElapsedTime));
        }

        protected void UpdateState(Player player)
        {
            RemovePlayerListeners(player);

            ResponseHandler.RegisterResponse(player);

            if (ResponseHandler.AllResponded())
                EndState();
        }
    }
}
