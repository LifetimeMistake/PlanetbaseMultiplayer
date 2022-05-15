using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Weather
{
    public struct Disaster
    {
        public float TimeRemaining;
        public float Intensity;
        public DisasterType Type;

        public Disaster(float timeRemaining, float intensity, DisasterType type)
        {
            TimeRemaining = timeRemaining;
            Intensity = intensity;
            Type = type;
        }
    }
}
