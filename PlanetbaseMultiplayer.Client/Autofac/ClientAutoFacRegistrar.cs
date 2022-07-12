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

namespace PlanetbaseMultiplayer.Client.Autofac
{
    public class ClientAutoFacRegistrar : IAutoFacRegistrar
    {
        private Client clientInstance;
        private GameStateMultiplayer gameStateMultiplayer;

        public ClientAutoFacRegistrar(Client clientInstance, GameStateMultiplayer gameStateMultiplayer)
        {
            this.clientInstance = clientInstance ?? throw new ArgumentNullException(nameof(clientInstance));
            this.gameStateMultiplayer = gameStateMultiplayer ?? throw new ArgumentNullException(nameof(gameStateMultiplayer));
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

            builder.RegisterInstance(clientInstance).As<Client>().ExternallyOwned();
            builder.RegisterInstance(gameStateMultiplayer).As<GameStateMultiplayer>().ExternallyOwned();
        }
    }
}
