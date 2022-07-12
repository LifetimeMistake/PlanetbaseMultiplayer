using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server
{
    public class ServerProcessorContext : IProcessorContext
    {
        public Server Server;
        public ServiceLocator ServiceLocator;

        public ServerProcessorContext(Server server, ServiceLocator serviceLocator)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
            ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
        }
    }
}
