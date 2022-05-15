using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets.Session;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.Simulation;
using PlanetbaseMultiplayer.Server.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Simulation
{
    public class SimulationManager : ISimulationManager
    {
        private Server server;
        private Player? simulationOwner;

        public bool IsInitialized { get; private set; }

        public event EventHandler<SimulationOwnerUpdatedEventArgs> SimulationOwnerUpdated;

        public SimulationManager(Server server)
        {
            this.server = server;
        }

        public bool Initialize()
        {
            if (server.PlayerManager == null)
                return false;

            server.PlayerManager.PlayerCreated += OnPlayerUpdated;
            server.PlayerManager.PlayerUpdated += OnPlayerUpdated;
            server.PlayerManager.PlayerRemoved += OnPlayerDestroyed;
            return true;
        }

        public Player? GetSimulationOwner()
        {
            return simulationOwner;
        }

        public bool SetSimulationOwner(Guid playerId)
        {
            Player player = server.PlayerManager.GetPlayer(playerId);
            return SetSimulationOwner(player);
        }

        public bool SetSimulationOwner(Player? player)
        {
            Guid? playerId;

            if (player != null)
            {
                if (player.Value.State != PlayerState.ConnectedReady)
                    return false;

                playerId = player.Value.Id;
                Console.WriteLine("New simulation owner: " + player.Value.Id);
            }
            else
            {
                playerId = null;
                Console.WriteLine("New simulation owner: None");
            }
            

            simulationOwner = player;

            SimulationOwnerChangedPacket simulationOwnerChangedPacket = new SimulationOwnerChangedPacket(playerId);
            server.SendPacketToAll(simulationOwnerChangedPacket);

            SimulationOwnerUpdatedEventArgs simulationOwnerUpdatedEventArgs = new SimulationOwnerUpdatedEventArgs(playerId);
            SimulationOwnerUpdated?.Invoke(this, simulationOwnerUpdatedEventArgs);

            return true;
        }

        private void OnPlayerUpdated(object sender, PlayerEventArgs player)
        {
            if (simulationOwner != null)
                return;

            FindNextSimulationOwner();
        }

        private void OnPlayerDestroyed(object sender, PlayerEventArgs player)
        {
            if (simulationOwner == null)
                return;

            if (player.PlayerId != simulationOwner.Value.Id)
                return;

            // ah fuck
            if(!FindNextSimulationOwner())
                SetSimulationOwner(null);
        }

        private bool FindNextSimulationOwner()
        {
            foreach(Player player in server.PlayerManager.GetPlayers().Where(player => player.State == PlayerState.ConnectedReady))
            {
                SetSimulationOwner(player);
                return true;
            }

            return false;
        }
    }
}
