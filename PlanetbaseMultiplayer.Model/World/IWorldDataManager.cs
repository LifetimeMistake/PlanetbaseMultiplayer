using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World
{
    public interface IWorldDataManager : IManager
    {
        bool RequestWorldData();
        void LoadWorldData(WorldData worldData);
        WorldData SaveWorldData();
    }
}
