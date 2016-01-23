using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol
{
    public interface IPlayer
    {
        string ID { get; set; }
        string Name { get; set; }
    }
}
