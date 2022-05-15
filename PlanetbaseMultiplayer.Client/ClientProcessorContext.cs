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

        public ClientProcessorContext(Client client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
}
