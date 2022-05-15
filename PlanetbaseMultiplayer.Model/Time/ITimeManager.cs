using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Time
{
    public interface ITimeManager : IManager
    {
        void SetNormalSpeed();
        void SetSpeed(float speed);
        void SetPausedState(bool paused);
        void SetTimescale(float speed, bool paused);
        void Pause();
        void Unpause();
        bool IsPaused();
        float GetCurrentSpeed();
        float GetReducedSpeed();

    }
}
