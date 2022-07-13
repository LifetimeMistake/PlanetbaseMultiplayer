using Autofac;
using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
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
using System.Reflection;
using System.Text;
using System.Threading;

namespace PlanetbaseMultiplayer.Server.Autofac
{
    public class ServerAutoFacRegistrar : IAutoFacRegistrar
    {
        private ServiceLocator serviceLocator;
        private ServerSettings serverSettings;

        public ServerAutoFacRegistrar(ServiceLocator serviceLocator, ServerSettings serverSettings)
        {
            this.serviceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
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

            builder.RegisterType<SynchronizationContext>().InstancePerLifetimeScope();
            builder.RegisterType<ProcessorContext>().InstancePerLifetimeScope();
            builder.RegisterType<PacketRouter>().InstancePerLifetimeScope();
            builder.RegisterType<Server>().InstancePerLifetimeScope();

            RegisterProcessors(builder);

            builder.RegisterInstance(serverSettings).As<ServerSettings>().ExternallyOwned();
            // this is extremely stupid and will have to be changed
            builder.RegisterInstance(serviceLocator).ExternallyOwned();
        }

        private void RegisterProcessors(ContainerBuilder builder)
        {
            foreach (PacketProcessor processor in PacketProcessor.GetProcessors())
                builder.RegisterType(processor.GetType()).As(typeof(PacketProcessor), processor.GetType()).InstancePerLifetimeScope();
        }
    }
}
