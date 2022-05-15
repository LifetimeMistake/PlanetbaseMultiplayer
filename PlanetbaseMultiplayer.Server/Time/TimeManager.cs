using PlanetbaseMultiplayer.Model.Packets.Time;
using PlanetbaseMultiplayer.Model.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Time
{
    public class TimeManager : ITimeManager
    {
        private Server server;
        public bool IsInitialized { get; private set; }
        private float timeScale;
        private bool isPaused;
        
        public TimeManager(Server server)
        {
            this.server = server;
        }

        public bool Initialize()
        {
            IsInitialized = true;
            return true;
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
            timeScale = speed;
            isPaused = paused;
            TimeScaleUpdatePacket timeScaleUpdatedPacket = new TimeScaleUpdatePacket(timeScale, isPaused);
            server.SendPacketToAll(timeScaleUpdatedPacket);
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
