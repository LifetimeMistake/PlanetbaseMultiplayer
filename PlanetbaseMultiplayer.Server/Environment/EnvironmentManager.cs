using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Math;
using PlanetbaseMultiplayer.Model.Packets.Environment;
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
        private Vector3D windDirection;

        public EnvironmentManager(Server server)
        {
            this.server = server;
            time = 0;
            windLevel = 0;
            windDirection = new Vector3D(1, 0, 0);
        }

        public void Initialize()
        {
            IsInitialized = true;
        }
        public float GetTimeOfDay()
        {
            return time;
        }

        public void SetTimeOfDay(float time)
        {
            UpdateEnvironmentData(time, windLevel, windDirection);
        }

        public void UpdateEnvironmentData(float time, float windLevel, Vector3D windDirection)
        {
            this.time = time;
            this.windLevel = windLevel;
            this.windDirection = windDirection;
            UpdateEnvironmentDataPacket updateEnvironmentDataPacket = new UpdateEnvironmentDataPacket(time, windLevel, windDirection);
            server.SendPacketToAll(updateEnvironmentDataPacket);
        }

        public float GetWindLevel()
        {
            return windLevel;
        }

        public void SetWindLevel(float windLevel)
        {
            UpdateEnvironmentData(time, windLevel, windDirection);
        }

        public Vector3D GetWindDirection()
        {
            return windDirection;
        }

        public void SetWindDirection(Vector3D windDirection)
        {
            UpdateEnvironmentData(time, windLevel, windDirection);
        }
    }
}
