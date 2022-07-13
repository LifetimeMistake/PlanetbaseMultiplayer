using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World
{
    public interface IWorldDataManager : IManager
    {
        bool RequestWorldData();
        void UpdateWorldData(WorldData worldData);
        WorldData GetWorldData();
    }
}
