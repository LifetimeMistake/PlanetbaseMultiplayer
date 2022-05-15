using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Time
{
    [Serializable]
    public class TimeScaleUpdatePacket : Packet
    {
        public float TimeScale;
        public bool IsPaused;

        public TimeScaleUpdatePacket(float timeScale, bool isPaused)
        {
            TimeScale = timeScale;
            IsPaused = isPaused;
        }
    }
}
