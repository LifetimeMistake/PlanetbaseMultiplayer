using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Environment.Disasters
{
    public interface IDisasterProxy
    {
        float Time { get; set; }
        float DisasterLength { get; set; }
        float Intensity { get; set; }
        void StartDisaster();
        void EndDisaster();
        void UpdateDisaster(float currentTime);
    }
}
