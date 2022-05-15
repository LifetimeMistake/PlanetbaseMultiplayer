using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets
{
    public enum ChannelType
    {
        ReliableOrdered = 0,
        ReliableUnordered = 1,
        Unreliable = 2
    }
}
