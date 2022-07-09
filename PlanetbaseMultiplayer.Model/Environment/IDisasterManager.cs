using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Environment
{
    public interface IDisasterManager : IManager, IPersistent
    {
        bool AnyDisasterInProgress();
        Disaster? GetDisasterInProgress();
        float GetCurrentTime();
        void CreateDisaster(Disaster disaster);
        void CreateDisaster(DisasterType disasterType, float disasterLength, float currentTime);
        void UpdateDisaster(float currentTime);
        void EndDisaster();
    }
}
