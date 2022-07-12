using Planetbase;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Math;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Environment
{
    public class EnvironmentManager : IEnvironmentManager
    {
        private Client client;
        private SimulationManager simulationManager;
        public bool IsInitialized { get; private set; }
        private float time;
        private float windLevel;
        private Vector3D windDirection;

        public EnvironmentManager(Client client, SimulationManager simulationManager)
        {
            this.client = client;
            this.simulationManager = simulationManager;
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

        public void OnUpdateEnvironmentData(float time, float windLevel, Vector3D windDirection)
        {
            this.time = time;
            this.windLevel = windLevel;
            this.windDirection = windDirection;

            Planetbase.EnvironmentManager environmentManager = Planetbase.EnvironmentManager.getInstance();
            Type environmentManagerType = environmentManager.GetType();

            FieldInfo mTimeIndicator = Reflection.GetPrivateFieldOrThrow(environmentManagerType, "mTimeIndicator", true);
            FieldInfo mWindIndicator = Reflection.GetPrivateFieldOrThrow(environmentManagerType, "mWindIndicator", true);
            FieldInfo mWindDirection = Reflection.GetPrivateFieldOrThrow(environmentManagerType, "mWindDirection", true);

            Indicator timeIndicator = (Indicator)Reflection.GetInstanceFieldValue(environmentManager, mTimeIndicator);
            Indicator windIndicator = (Indicator)Reflection.GetInstanceFieldValue(environmentManager, mWindIndicator);

            timeIndicator.setValue(time);
            windIndicator.setValue(windLevel);
            Reflection.SetInstanceFieldValue(environmentManager, mWindDirection, (Vector3)windDirection);

        }

        public void UpdateEnvironmentData(float time, float windLevel, Vector3D windDirection)
        {
            Player? simulationOwner = simulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != client.LocalPlayer)
                return; // Don't send the packet if we aren't the simulation owner

            UpdateEnvironmentDataPacket updateEnvironmentDataPacket = new UpdateEnvironmentDataPacket(time, windLevel, windDirection);
            client.SendPacket(updateEnvironmentDataPacket);
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