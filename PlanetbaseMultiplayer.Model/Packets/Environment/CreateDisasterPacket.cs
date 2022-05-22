using PlanetbaseMultiplayer.Model.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Environment
{
    [Serializable]
    public class CreateDisasterPacket : Packet
    {
        public Disaster Disaster;

        public CreateDisasterPacket(Disaster disaster)
        {
            Disaster = disaster;
        }
    }
}
