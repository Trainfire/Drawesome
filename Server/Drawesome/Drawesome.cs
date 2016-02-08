using Server.Game;
using System.Collections.Generic;
using System;
using Protocol;

namespace Server.Drawesome
{
    public class Drawesome : Game.Game
    {
        public Drawesome(List<Player> players) 
            : base (players)
        {
            AddState(new Drawing());
            AddState(new Answering());
        }
    }

    #region States

    public class Drawing : State
    {
        public override GameState Type { get { return GameState.Drawing; } }

        public Drawing()
        {

        }

        protected override void OnBegin()
        {
            throw new NotImplementedException();
        }

        public override void OnMessage(Message message)
        {
            
        }
    }

    public class Answering : State
    {
        public override GameState Type { get { return GameState.Answering; } }

        public Answering()
        {

        }

        protected override void OnBegin()
        {
            
        }

        public override void OnMessage(Message message)
        {
            base.OnMessage(message);
        }
    }

    #endregion
}
