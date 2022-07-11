using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client.Simulation
{
    public class SimulationManager : ISimulationManager
    {
        private Client client;
        private Player? simulationOwner;

        public bool IsInitialized { get; private set; }

        public SimulationManager(Client client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void Initialize()
        {
            IsInitialized = true;
        }

        public Player? GetSimulationOwner()
        {
            return simulationOwner;
        }

        public void OnSimulationOwnerUpdated(Player? player)
        {
            simulationOwner = player;
        }
    }
}
