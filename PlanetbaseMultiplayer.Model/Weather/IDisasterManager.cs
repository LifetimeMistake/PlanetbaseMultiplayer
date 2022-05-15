using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Weather
{
    public interface IDisasterManager
    {
        Disaster? CurrentDisaster { get; set; }

        bool AnyDisasterInProgress();
        Disaster? GetDisasterInProgress();
        void CreateDisaster(Disaster disaster);
        void CreateDisaster(DisasterType disasterType, float disasterLength, float instensity);
        void UpdateDisaster(float timeScale);
        void EndDisaster();
    }
}
