using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets.Time;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Client.Time
{
    public class TimeManager : ITimeManager
    {
        private Client client;
        public bool IsInitialized { get; private set; }
        private float timeScale;
        private bool isPaused;
        
        public TimeManager(Client client)
        {
            this.client = client;
        }
        public bool Initialize()
        {
            IsInitialized = true;
            return true;
        }

        public void OnTimeScaleUpdated(float timeScale, bool paused)
        {
            this.timeScale = timeScale;
            isPaused = paused;

            FieldInfo mPaused;
            MethodInfo OnTimeScaleChanged;

            Planetbase.TimeManager timeManager = Planetbase.TimeManager.getInstance();
            if (!Reflection.TryGetPrivateField(timeManager.GetType(), "mPaused", true, out mPaused)) 
            {
                Debug.LogError("Failed to find \"mPaused\"");
                return;
            }

            if (!Reflection.TryGetPrivateMethod(timeManager.GetType(), "onTimeScaleChanged", true, out OnTimeScaleChanged))
            {
                Debug.LogError("Failed to find \"onTimeScaleChanged\"");
                return;
            }

            Reflection.SetInstanceFieldValue(timeManager, mPaused, isPaused);
            Reflection.InvokeInstanceMethod(timeManager, OnTimeScaleChanged, new object[] { });
        }

        public void SetNormalSpeed()
        {
            SetSpeed(1f);
        }

        public void SetSpeed(float speed)
        {
            SetTimescale(speed, isPaused);
        }

        public void SetPausedState(bool paused)
        {
            SetTimescale(timeScale, paused);
        }

        public void SetTimescale(float speed, bool paused)
        {
            Player? simulationOwner = client.SimulationManager.GetSimulationOwner();
            if (simulationOwner == null || simulationOwner.Value != client.LocalPlayer)
                return; // Don't send the packet if we aren't the simulation owner

            TimeScaleUpdatePacket timeScaleUpdatePacket = new TimeScaleUpdatePacket(speed, paused);
            client.SendPacket(timeScaleUpdatePacket);
        }

        public void Pause()
        {
            SetPausedState(true);
        }

        public void Unpause()
        {
            SetPausedState(false);
        }

        public bool IsPaused()
        {
            return isPaused;
        }

        public float GetCurrentSpeed()
        {
            return timeScale;
        }

        public float GetReducedSpeed()
        {
            return (float)Math.Sqrt(timeScale);
        }

    }
}
