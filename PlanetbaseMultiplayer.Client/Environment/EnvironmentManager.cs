﻿using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Math;
using PlanetbaseMultiplayer.Model.Packets.Environment;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Environment
{
    public class EnvironmentManager : IEnvironmentManager
    {
        private Client client;
        public bool IsInitialized { get; private set; }
        private float time;
        private float windLevel;
        private Vector3D windDirection;

        public EnvironmentManager(Client client)
        {
            this.client = client;
            time = 0;
            windLevel = 0;
            windDirection = new Vector3D(1, 0, 0);
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
            UpdateEnvironmentData(time, windLevel, windDirection);
        }

        public void OnUpdateEnvironmentData(float time, float windLevel)
        {
            this.time = time;
            this.windLevel = windLevel;
            // do stuff
        }

        public void UpdateEnvironmentData(float time, float windLevel, Vector3D windDirection)
        {
            Player? simulationOwner = client.SimulationManager.GetSimulationOwner();
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