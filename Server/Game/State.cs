using System;
using Protocol;

namespace Server.Game
{
    public abstract class State<T> where T : GameData
    {
        public event EventHandler<T> OnEnd;

        public abstract GameState Type { get; }
        public T GameData { get; protected set; }

        public void Begin(T gameData)
        {
            GameData = gameData;
            OnBegin();
        }

        protected virtual void OnBegin()
        {

        }

        public virtual void OnMessage(string json)
        {

        }

        protected void EndState()
        {
            if (OnEnd != null)
                OnEnd(this, GameData);
        }
    }
}
