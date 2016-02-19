using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IConnectionMessageHandler
    {
        void HandleMessage(Player player, string json);
    }
}
