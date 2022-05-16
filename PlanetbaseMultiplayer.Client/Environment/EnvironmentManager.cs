using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Environment
{
    public class EnvironmentManager : IEnvironmentManager
    {
        private Client client;
        public bool IsInitialized { get; private set; }
        private float time;
        private float windLevel;

        public EnvironmentManager(Client client)
        {
            this.client = client;
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

        public void OnUpdateEnvironmentData(float time, float windLevel)
        {
            this.time = time;
            this.windLevel = windLevel;
            // do stuff
        }

        public void UpdateEnvironmentData(float time, float windLevel)
        {
            Player? simulationOwner = client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != client.LocalPlayer)
                return; // Don't send the packet if we aren't the simulation owner

            UpdateEnvironmentDataPacket updateEnvironmentDataPacket = new UpdateEnvironmentDataPacket(time, windLevel);
            client.SendPacket(updateEnvironmentDataPacket);
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
