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
        void StartDisaster();
        void EndDisaster();
        void UpdateDisaster(float timeStep);
    }
}
