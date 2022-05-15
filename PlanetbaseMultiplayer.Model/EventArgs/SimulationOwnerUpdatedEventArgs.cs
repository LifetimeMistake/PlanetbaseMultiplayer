using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.EventArgs
{
    public class SimulationOwnerUpdatedEventArgs : System.EventArgs
    {
        public Guid? SimulationOwnerId;

        public SimulationOwnerUpdatedEventArgs(Guid? simulationOwnerId)
        {
            SimulationOwnerId = simulationOwnerId;
        }
    }
}
