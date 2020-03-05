using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client
{
    public class MultiplayerResourceManager
    {
        private Dictionary<Guid, Resource> resources;
        private Client gameClient;

        public MultiplayerResourceManager(Client gameClient)
        {
            this.gameClient = gameClient;
            resources = new Dictionary<Guid, Resource>();
        }

        public void AddResource(Resource resource)
        {
            Guid guid = Guid.NewGuid();
        }

        public void RemoveResource(Resource resource)
        {

        }
    }
}
