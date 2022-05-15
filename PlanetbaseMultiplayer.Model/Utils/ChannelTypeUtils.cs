using Lidgren.Network;
using PlanetbaseMultiplayer.Model.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Utils
{
    public static class ChannelTypeUtils
    {
        public static NetDeliveryMethod ChannelTypeToLidgren(ChannelType channelType)
        {
            switch (channelType)
            {
                case ChannelType.ReliableOrdered:
                    return NetDeliveryMethod.ReliableOrdered;
                case ChannelType.ReliableUnordered:
                    return NetDeliveryMethod.ReliableUnordered;
                case ChannelType.Unreliable:
                    return NetDeliveryMethod.Unreliable;
                default:
                    return NetDeliveryMethod.ReliableOrdered;
            }
        }
    }
}
