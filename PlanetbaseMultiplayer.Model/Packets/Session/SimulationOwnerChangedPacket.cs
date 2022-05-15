using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Session
{
    [Serializable]
    public class SimulationOwnerChangedPacket : Packet
    {
        public Guid? PlayerId;

        public SimulationOwnerChangedPacket(Guid? playerId)
        {
            PlayerId = playerId;
        }
    }
}
