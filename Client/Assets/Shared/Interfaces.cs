using System;
using System.Collections.Generic;
using Protocol;

namespace Protocol
{
    public interface IClientSettings
    {
        string HostURL { get; set; }
        float AfkTimer { get; set; }
    }
}
