using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World
{
    public interface IWorldSaveManager : IManager
    {
        bool RequestWorldData();
        void UpdateWorldData(WorldStateData worldStateData);
        WorldStateData GetWorldData();
    }
}
