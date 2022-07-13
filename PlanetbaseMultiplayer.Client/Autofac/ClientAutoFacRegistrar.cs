using Autofac;
using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Model.Environment;
using PlanetbaseMultiplayer.Model.Simulation;
using PlanetbaseMultiplayer.Model.Time;
using PlanetbaseMultiplayer.Model.World;
using PlanetbaseMultiplayer.Client.Environment;
using PlanetbaseMultiplayer.Client.Players;
using PlanetbaseMultiplayer.Client.Simulation;
using PlanetbaseMultiplayer.Client.Time;
using PlanetbaseMultiplayer.Client.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseMultiplayer.Client.Timers;
using PlanetbaseMultiplayer.Client.Debugging;
using PlanetbaseMultiplayer.Client.GameStates;
using PlanetbaseMultiplayer.Model.Packets.Processors.Abstract;
using System.Reflection;
using System.Threading;
using PlanetbaseMultiplayer.Model.Packets;
using PlanetbaseMultiplayer.Model.Packets.Processors;

namespace PlanetbaseMultiplayer.Client.Autofac
{
    public class ClientAutoFacRegistrar : IAutoFacRegistrar
    {
        // this is stupid and will have to be changed somehow
        private ServiceLocator serviceLocator;
        private GameStateMultiplayer multiplayerScene;

        public ClientAutoFacRegistrar(ServiceLocator serviceLocator, GameStateMultiplayer multiplayerScene)
        {
            this.serviceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
            this.multiplayerScene = multiplayerScene ?? throw new ArgumentNullException(nameof(multiplayerScene));
        }

        public void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<PlayerManager>().InstancePerLifetimeScope();
            builder.RegisterType<SimulationManager>().InstancePerLifetimeScope();
            builder.RegisterType<WorldStateManager>().InstancePerLifetimeScope();
            builder.RegisterType<TimeManager>().InstancePerLifetimeScope();
            builder.RegisterType<EnvironmentManager>().InstancePerLifetimeScope();
            builder.RegisterType<DisasterManager>().InstancePerLifetimeScope();

#if DEBUG
            builder.RegisterType<DebugManager>().InstancePerLifetimeScope();
#endif

            builder.RegisterType<ProcessorContext>().InstancePerLifetimeScope();
            builder.RegisterType<PacketRouter>().InstancePerLifetimeScope();
            builder.RegisterType<SynchronizationContext>().InstancePerLifetimeScope();
            builder.RegisterType<TimerActionManager>().InstancePerLifetimeScope();
            builder.RegisterType<GameStateMultiplayer>().InstancePerLifetimeScope();
            builder.RegisterType<Client>().InstancePerLifetimeScope();

            RegisterProcessors(builder);

            // this is moderately stupid
            builder.RegisterInstance(multiplayerScene).ExternallyOwned();
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
