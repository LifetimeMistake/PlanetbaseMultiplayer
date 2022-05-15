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

        public ServerProcessorContext(Server server)
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
        }
    }
}
