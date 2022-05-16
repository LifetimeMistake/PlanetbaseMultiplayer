using PlanetbaseMultiplayer.Model.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Environment
{
    public class EnvironmentManager : IEnvironmentManager
    {
        private Server server;
        public bool IsInitialized { get; private set; }
        private float time;
        private float windLevel;

        public EnvironmentManager(Server server)
        {
            this.server = server;
        }

        public bool Initialize()
        {
            IsInitialized = true;
            return true;
        }
        public float GetTimeOfDay()
        {
            return time;
        }

        public void SetTimeOfDay(float time)
        {
            UpdateEnvironmentData(time, windLevel);
        }

        public void UpdateEnvironmentData(float time, float windLevel)
        {
            this.time = time;
            this.windLevel = windLevel;
        }

        public float GetWindLevel()
        {
            return windLevel;
        }

        public void SetWindLevel(float windLevel)
        {
            UpdateEnvironmentData(time, windLevel);
        }
    }
}
