using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World
{
    public interface IWorldStateManager : IManager
    {
        bool RequestWorldData();
        void UpdateWorldData(WorldData worldStateData);
        WorldData GetWorldData();
    }
}
