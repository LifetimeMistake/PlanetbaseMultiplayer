using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.World;
using PlanetbaseMultiplayer.Server.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.World
{
    public class WorldStateManager : IWorldStateManager
    {
        private WorldData worldStateData;
        private ServerSettings serverSettings;
        private SimulationManager simulationManager;
        private WorldStateData worldStateData;
        private Server server;
        private bool dataRequestInProgress;

        public WorldStateManager(ServerSettings serverSettings, SimulationManager simulationManager, Server server)
        {
            this.serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
            this.simulationManager = simulationManager ?? throw new ArgumentNullException(nameof(simulationManager));
            this.server = server ?? throw new ArgumentNullException(nameof(server));
        }

        public bool IsInitialized { get; private set; }

        public event EventHandler WorldDataRequestSent;
        public event EventHandler WorldDataUpdated;
        public event EventHandler WorldDataRequestFailed;

        public WorldStateManager(Server server, string savePath, WorldStateData worldStateData)
        {
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            this.savePath = savePath ?? throw new ArgumentNullException(nameof(savePath));
            this.worldStateData = worldStateData;

            simulationManager.SimulationOwnerUpdated += OnSimulationOwnerUpdated;
            IsInitialized = true;
        }

        public bool RequestWorldData()
        {
            Player? player = simulationManager.GetSimulationOwner();
            if (player == null)
                return false; // Can't request world data, there are no simulation owners

            if (dataRequestInProgress)
                return true;

            Console.WriteLine("Requesting world data...");
            WorldDataRequestSent?.Invoke(this, new System.EventArgs());
            dataRequestInProgress = true;
            WorldDataRequestPacket worldDataRequestPacket = new WorldDataRequestPacket();
            server.SendPacketToPlayer(worldDataRequestPacket, player.Value.Id);
            return true;
        }

        public void UpdateWorldData(WorldData worldStateData)
        {
            this.worldStateData = worldStateData;
            try
            {
                File.WriteAllText(serverSettings.SavePath, worldStateData.XmlData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write the world data to disk: {ex}");
            }

            WorldDataUpdated?.Invoke(this, new System.EventArgs());
        }

        public WorldData GetWorldData()
        {
            return worldStateData;
        }
        
        public void OnWorldDataReceived(WorldData worldStateData)
        {
            Console.WriteLine("Received world data, updating...");
            dataRequestInProgress = false;
            UpdateWorldData(worldStateData);
        }

        private void OnSimulationOwnerUpdated(object sender, EventArgs.SimulationOwnerUpdatedEventArgs e)
        {
            if (!dataRequestInProgress)
                return;

            // Handle edge cases
            // A simulation owner may disconnect while a request is in progress
            // In this case we attempt to resend it
            dataRequestInProgress = false;
            if (!RequestWorldData())
            {
                // If there are no simulation owners present, we let event listeners know that the previous request failed.
                Console.WriteLine("World data request failed");
                WorldDataRequestFailed?.Invoke(this, new System.EventArgs());
            }
        }
    }
}
