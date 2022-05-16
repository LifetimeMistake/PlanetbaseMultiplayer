using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Environment
{
    [Serializable]
    public class UpdateEnvironmentDataPacket : Packet
    {
        public float Time;
        public float WindLevel;

        public UpdateEnvironmentDataPacket(float time, float windLevel)
        {
            this.Time = time;
            this.WindLevel = windLevel;
        }

    }
}
