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
        void Pause();
        void Unpause();
        float GetCurrentSpeed();
        float GetReducedSpeed();
    }
}
