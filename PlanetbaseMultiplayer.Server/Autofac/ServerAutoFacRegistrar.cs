using Autofac;
using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Simulation;
using PlanetbaseMultiplayer.Model.Time;
using PlanetbaseMultiplayer.Model.World;
using PlanetbaseMultiplayer.Server.Environment;
using PlanetbaseMultiplayer.Server.Players;
using PlanetbaseMultiplayer.Server.Simulation;
using PlanetbaseMultiplayer.Server.Time;
using PlanetbaseMultiplayer.Server.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Server.Autofac
{
    public class ServerAutoFacRegistrar : IAutoFacRegistrar
    {
        private Server serverInstance;
        private ServerSettings serverSettings;

        public ServerAutoFacRegistrar(Server serverInstance, ServerSettings serverSettings)
        {
            this.serverInstance = serverInstance ?? throw new ArgumentNullException(nameof(serverInstance));
            this.serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
        }

        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<PlayerManager>().InstancePerLifetimeScope();
            builder.RegisterType<SimulationManager>().InstancePerLifetimeScope();
            builder.RegisterType<WorldStateManager>().InstancePerLifetimeScope();
            builder.RegisterType<WorldRequestQueueManager>().InstancePerLifetimeScope();
            builder.RegisterType<TimeManager>().InstancePerLifetimeScope();
            builder.RegisterType<EnvironmentManager>().InstancePerLifetimeScope();
            builder.RegisterType<DisasterManager>().InstancePerLifetimeScope();

            builder.RegisterInstance(serverSettings).As<ServerSettings>().ExternallyOwned();
            builder.RegisterInstance(serverInstance).As<Server>().ExternallyOwned();
        }
    }
}
