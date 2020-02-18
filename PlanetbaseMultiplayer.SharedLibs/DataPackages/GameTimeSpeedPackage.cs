using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class GameTimeSpeedPackage : IDataPackage
    {
        public bool Paused;
        public GameTimeSpeed GameSpeed;

        public GameTimeSpeedPackage(bool paused, GameTimeSpeed gameSpeed)
        {
            Paused = paused;
            GameSpeed = gameSpeed;
        }
    }
}
