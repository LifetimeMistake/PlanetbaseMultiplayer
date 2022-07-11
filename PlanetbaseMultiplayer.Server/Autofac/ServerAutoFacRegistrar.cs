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
        private Server ownerInstance;

        public ServerAutoFacRegistrar(Server ownerInstance)
        {
            this.ownerInstance = ownerInstance ?? throw new ArgumentNullException(nameof(ownerInstance));
        }

        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<PlayerManager>();
            builder.RegisterType<SimulationManager>().As<ISimulationManager>();
            builder.RegisterType<WorldStateManager>().As<IWorldStateManager>();
            builder.RegisterType<WorldRequestQueueManager>();
            builder.RegisterType<TimeManager>().As<ITimeManager>();
            builder.RegisterType<EnvironmentManager>().As<IEnvironmentManager>();
            builder.RegisterType<DisasterManager>().As<IDisasterManager>();
            builder.RegisterInstance(ownerInstance).As<Server>().ExternallyOwned();
        }
    }
}
