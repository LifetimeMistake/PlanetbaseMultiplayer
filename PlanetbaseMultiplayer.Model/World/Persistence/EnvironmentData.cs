using PlanetbaseMultiplayer.Model.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World.Persistence
{
    [Serializable]
    public class EnvironmentData
    {
        public float DayTime;
        public float WindLevel;
        public Vector3D WindDirection;

        public EnvironmentData(float dayTime, float windLevel, Vector3D windDirection)
        {
            DayTime = dayTime;
            WindLevel = windLevel;
            WindDirection = windDirection;
        }
    }
}
