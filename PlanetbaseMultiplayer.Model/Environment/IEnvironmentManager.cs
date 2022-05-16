using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Environment
{
    public interface IEnvironmentManager : IManager
    {
        float GetTimeOfDay();
        void SetTimeOfDay(float time);
        void UpdateEnvironmentData(float time, float windLevel);
        float GetWindLevel();
        void SetWindLevel(float windLevel);
    }
}
