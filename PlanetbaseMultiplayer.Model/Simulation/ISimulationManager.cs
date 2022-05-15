using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Simulation
{
    public interface ISimulationManager : IManager
    {
        Player? GetSimulationOwner();
    }
}
