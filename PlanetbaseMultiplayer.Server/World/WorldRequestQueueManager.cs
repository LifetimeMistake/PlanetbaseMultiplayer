using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.World
{
    public class WorldRequestQueueManager : IManager
    {
        private Server server;
        public bool IsInitialized { get; private set; }

        public WorldRequestQueueManager(Server server)
        {
            this.server = server ?? throw new ArgumentNullException(nameof(server));
        }

        public bool Initialize()
        {
            server.WorldStateManager.WorldDataUpdated += OnWorldDataUpdated;
            server.WorldStateManager.WorldDataRequestFailed += OnWorldDataRequestFailed;
            IsInitialized = true;
            return true;
        }

        public bool EnqueuePlayer(Player player)
        {
            throw new NotImplementedException();
        }

        // Removes the player from the queue without sending data
        public bool DropPlayer(Player player)
        {
            throw new NotImplementedException();
        }

        private void OnWorldDataUpdated(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnWorldDataRequestFailed(object sender, System.EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
