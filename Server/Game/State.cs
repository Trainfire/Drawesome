using System;
using Protocol;

namespace Server.Game
{
    public abstract class State
    {
        public event EventHandler OnEnd;

        public abstract GameState Type { get; }

        public void Begin()
        {
            OnBegin();
        }

        protected virtual void OnBegin()
        {

        }

        public virtual void OnMessage(Message message)
        {

        }

        protected void EndState()
        {
            if (OnEnd != null)
                OnEnd(this, null);
        }
    }
}
