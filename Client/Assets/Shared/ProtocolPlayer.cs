using System;
using Newtonsoft.Json;

namespace Protocol
{
    public class ProtocolPlayer
    {
        public virtual string ID { get; set; }
        public virtual string Name { get; set; }
    }

    public class ProtocolRoom
    {
        public virtual string ID { get; set; }
    }
}
