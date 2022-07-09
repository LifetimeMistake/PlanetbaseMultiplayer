using PlanetbaseMultiplayer.Model.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World.Persistence
{
    [Serializable]
    public class DisasterData
    {
        public Disaster? CurrentDisaster;

        public DisasterData(Disaster? currentDisaster)
        {
            CurrentDisaster = currentDisaster;
        }
    }
}
