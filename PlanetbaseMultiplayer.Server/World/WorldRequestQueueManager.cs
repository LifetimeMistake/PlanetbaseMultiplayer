using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Packets.World;
using PlanetbaseMultiplayer.Model.Players;
using PlanetbaseMultiplayer.Model.World;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.World
{
    public class WorldRequestQueueManager : IManager
    {
        private Server server;
        private ConcurrentDictionary<Guid, Player> playerQueue;
        public bool IsInitialized { get; private set; }

        public WorldRequestQueueManager(Server server)
        {
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            this.playerQueue = new ConcurrentDictionary<Guid, Player>();
        }

        public bool Initialize()
        {
            server.WorldStateManager.WorldDataUpdated += OnWorldDataUpdated;
            server.WorldStateManager.WorldDataRequestFailed += OnWorldDataRequestFailed;
            server.PlayerManager.PlayerRemoved += OnPlayerRemoved;
            IsInitialized = true;
            return true;
        }

        public bool EnqueuePlayer(Player player)
        {
            return playerQueue.TryAdd(player.Id, player);
        }

        // Removes the player from the queue without sending data
        public bool DropPlayer(Player player)
        {
            return playerQueue.TryRemove(player.Id, out _);
        }

        private void OnWorldDataUpdated(object sender, System.EventArgs e)
        {
            // The server completed a world data request successfully
            // We are for sure using the newest available world state
            DeliverWorldData();
        }

        private void OnWorldDataRequestFailed(object sender, System.EventArgs e)
        {
            // The server failed multiple world data requests
            // The data on the server is now for sure the newest world state
            // the other players must have lost connection
            DeliverWorldData();
        }

        private void DeliverWorldData()
        {
            WorldStateData worldStateData = server.WorldStateManager.GetWorldData();
            WorldDataPacket worldDataPacket = new WorldDataPacket(worldStateData);
            foreach (KeyValuePair<Guid, Player> kvp in playerQueue)
            {
                server.SendPacketToPlayer(worldDataPacket, kvp.Key);
            }

            playerQueue.Clear();
        }

        private void OnPlayerRemoved(object sender, EventArgs.PlayerEventArgs e)
        {
            playerQueue.TryRemove(e.PlayerId, out _);
        }
    }
}
