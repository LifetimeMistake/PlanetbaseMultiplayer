using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Weather
{
    public interface IEnvironmentManager : IManager
    {
        float GetTimeOfDay();
        void SetTimeOfDay(float time);
        float GetWindLevel();
        void SetWindLevel(float windLevel);
    }
}
