using System;
using System.Collections.Generic;

namespace Protocol
{
    [Serializable]
    public class PlayerData
    {
        public string ID;
        public string Name;
    }

    public class RoomData
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public List<PlayerData> Players { get; set; }
    }
}
