using PlanetbaseMultiplayer.Model.Math;
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
        float GetWindLevel();
        void SetWindLevel(float windLevel);
        Vector3D GetWindDirection();
        void SetWindDirection(Vector3D windDirection);
        void UpdateEnvironmentData(float time, float windLevel, Vector3D windDirection);
    }
}
