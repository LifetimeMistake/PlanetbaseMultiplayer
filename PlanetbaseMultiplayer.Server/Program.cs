using PlanetbaseMultiplayer.Model.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server
{
    public static class Program
    {
        private static Server server;
        public static void Main()
        {
            ServerSettings serverSettings = new ServerSettings("gaming", "aaa", 8081, "save.sav");
            server = new Server(serverSettings);
            server.Start();
            Console.ReadLine();
        }
    }
}
