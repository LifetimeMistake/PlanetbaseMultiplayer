using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Server.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server
{
    public static class Program
    {
        public static void Main()
        {
            ServerSettings serverSettings = new ServerSettings("gaming", "aaa", 8081, "save.sav");
            ServiceLocator serviceLocator = new ServiceLocator();
            ServerAutoFacRegistrar serverAutoFacRegistrar = new ServerAutoFacRegistrar(serviceLocator, serverSettings);
            serviceLocator.Initialize(serverAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            Server server = serviceLocator.LocateService<Server>();
            server.Start();
            server.Initialize();
            Console.ReadLine();
        }
    }
}
