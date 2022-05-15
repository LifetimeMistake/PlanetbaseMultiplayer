using PlanetbaseMultiplayer.Model.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Session
{
    [Serializable]
    public class DisconnectRequestPacket : Packet
    {
        public DisconnectReason Reason;

        public DisconnectRequestPacket(DisconnectReason reason)
        {
            Reason = reason;
        }
    }
}
