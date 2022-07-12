using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Client
{
    public class ClientProcessorContext : IProcessorContext
    {
        public Client Client;
        public ServiceLocator ServiceLocator;

        public ClientProcessorContext(Client client, ServiceLocator serviceLocator)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
        }
    }
}
