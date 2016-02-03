using System;
using Newtonsoft.Json;

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
    }
}
