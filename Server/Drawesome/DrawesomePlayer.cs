using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Drawesome
{
    public class DrawesomePlayer
    {
        public uint Score { get; private set; }
        public uint Likes { get; private set; }

        public void AddScore(uint score)
        {
            Score += score;
        }

        public void AddLike()
        {
            Likes++;
        }
    }
}
