using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Environment
{
    [Serializable]
    public class UpdateDisasterPacket : Packet
    {
        public float CurrentTime;

        public UpdateDisasterPacket(float currentTime)
        {
            CurrentTime = currentTime;
        }
    }
}
