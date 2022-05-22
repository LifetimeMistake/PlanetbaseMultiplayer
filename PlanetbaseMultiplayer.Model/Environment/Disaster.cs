using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Environment
{
    [Serializable]
    public struct Disaster
    {
        private float currentTime;
        private float disasterLength;
        private DisasterType type;

        public float CurrentTime { get => currentTime; set => currentTime = value; }
        public float DisasterLength { get => disasterLength; set => disasterLength = value; }
        public DisasterType Type { get => type; set => type = value; }

        public Disaster(DisasterType type, float disasterLength, float currentTime)
        {
            this.disasterLength = disasterLength;
            this.type = type;
            this.currentTime = currentTime;
        }
    }
}
