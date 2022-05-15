using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Session
{
    [Serializable]
    public class PlayerDisconnectedPacket : Packet
    {
        public Guid PlayerId;
        public DisconnectReason Reason;

        public PlayerDisconnectedPacket(Guid playerId, DisconnectReason reason)
        {
            PlayerId = playerId;
            Reason = reason;
        }
    }
}
