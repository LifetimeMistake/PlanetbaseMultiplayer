
using PlanetbaseMultiplayer.Model.Packets.Time;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Time;
using PlanetbaseMultiplayer.Server.Players;
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
        private bool timeLocked;
        
        public TimeManager(Server server)
        {
            this.server = server;
            timeScale = 1f;
            isPaused = false;
        }

        public void Initialize()
        {
            IsInitialized = true;
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
            if (timeLocked)
                return;

            timeScale = speed;
            isPaused = paused;

            TimeScaleUpdatePacket timeScaleUpdatedPacket = new TimeScaleUpdatePacket(timeScale, isPaused);
            server.SendPacketToAll(timeScaleUpdatedPacket);
        }

        public void FreezeTime()
        {
            if (timeLocked)
                return;

            TimeScaleUpdatePacket timeScaleUpdatedPacket = new TimeScaleUpdatePacket(timeScale, true);
            server.SendPacketToAll(timeScaleUpdatedPacket);
            timeLocked = true;
        }

        public void UnfreezeTime()
        {
            if (!timeLocked)
                return;

            timeLocked = false;
            SetPausedState(isPaused);
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

        public bool IsTimeLocked()
        {
            return timeLocked;
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
