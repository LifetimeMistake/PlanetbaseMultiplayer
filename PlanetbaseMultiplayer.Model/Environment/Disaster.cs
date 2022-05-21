using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Environment
{
    public struct Disaster
    {
        public float CurrentTime;
        public float DisasterLength;
        public DisasterType Type;

        public Disaster(DisasterType type, float disasterLength, float currentTime)
        {
            DisasterLength = disasterLength;
            Type = type;
            CurrentTime = currentTime;
        }
    }
}
