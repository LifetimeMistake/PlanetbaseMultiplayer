using PlanetbaseMultiplayer.Model.Math;
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
        public Vector3D WindDirection;

        public UpdateEnvironmentDataPacket(float time, float windLevel, Vector3D windDirection)
        {
            this.Time = time;
            this.WindLevel = windLevel;
            this.WindDirection = windDirection;
        }

    }
}
