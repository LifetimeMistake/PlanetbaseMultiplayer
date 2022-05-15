using PlanetbaseMultiplayer.Model.Packets.Time;
using PlanetbaseMultiplayer.Model.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            //chuja daje
        }

        public void SetNormalSpeed()
        {
            SetSpeed(1f);
        }

        public void SetSpeed(float speed)
        {
            SendRequest(speed, isPaused);
        }

        public void SetPausedState(bool paused)
        {
            SendRequest(timeScale, paused);
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

        private void SendRequest(float speed, bool paused)
        {
            TimeScaleUpdatePacket timeScaleUpdatePacket = new TimeScaleUpdatePacket(speed, paused);
            client.SendPacket(timeScaleUpdatePacket);
        }


    }
}
