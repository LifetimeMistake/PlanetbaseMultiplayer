using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Autofac;
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
    public class WorldDataManager : IWorldDataManager
    {
        private ServiceLocator serviceLocator;
        private ServerSettings serverSettings;
        private SimulationManager simulationManager;
        private WorldData worldData;
        private Server server;
        private bool dataRequestInProgress;

        public bool IsInitialized { get; private set; }

        public event EventHandler WorldDataRequestSent;
        public event EventHandler WorldDataUpdated;
        public event EventHandler WorldDataRequestFailed;

        public WorldDataManager(ServiceLocator serviceLocator, ServerSettings serverSettings, SimulationManager simulationManager, Server server)
        {
            this.serviceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
            this.serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
            this.simulationManager = simulationManager ?? throw new ArgumentNullException(nameof(simulationManager));
            this.server = server ?? throw new ArgumentNullException(nameof(server));
        }

        public void Initialize()
        {
            using (FileStream stream = File.OpenRead(serverSettings.SavePath))
            {
                worldData = WorldData.Deserialize(stream);
            }

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

        public void LoadWorldData(WorldData worldData)
        {
            this.worldData = worldData;
            try
            {
                using(FileStream file = File.OpenWrite(serverSettings.SavePath))
                {
                    worldData.Serialize(file);
                }
            }
            catch (Exception ex)
            {
                // This shouldn't be happening but is non-fatal
                Console.WriteLine($"Failed to write the world data to disk: {ex}");
            }

            foreach(IPersistent persistent in serviceLocator.LocateServicesOfType<IPersistent>())
            {
                try
                {
                    persistent.Load(worldData);
                }
                catch(Exception ex)
                {
                    throw new Exception($"Failed to load world data into persistent manager \"{persistent.GetType().Name}\": {ex}", ex);
                }
            }

            WorldDataUpdated?.Invoke(this, new System.EventArgs());
        }

        public WorldData SaveWorldData()
        {
            // TODO: Implement server-side saving to get rid of sim owner world data requests
            return worldData;
        }
        
        public void OnWorldDataReceived(WorldData worldData)
        {
            Console.WriteLine("Received world data, updating...");
            dataRequestInProgress = false;
            LoadWorldData(worldData);
        }

        public void OnWorldRequestFailed()
        {
            if(dataRequestInProgress)
            {
                dataRequestInProgress = false;
                WorldDataRequestFailed?.Invoke(this, new System.EventArgs());
            }
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
