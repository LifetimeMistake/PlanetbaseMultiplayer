using NUnit.Framework;
using Planetbase;
using PlanetbaseMultiplayer.Client.Autofac;
using PlanetbaseMultiplayer.Client.GameStates;
using PlanetbaseMultiplayer.Model.Autofac;
using System.Runtime.Serialization;
using System.Linq;

namespace PlanetbaseMultiplayer.Client.Tests
{
    public class AutofacTests
    {
        [Test]
        public void ResolveClientTest()
        {
            GameStateMultiplayer gameStateMultiplayer = FormatterServices.GetUninitializedObject(typeof(GameStateMultiplayer)) as GameStateMultiplayer;
            ServiceLocator serviceLocator = new ServiceLocator();

            ClientAutoFacRegistrar clientAutoFacRegistrar = new ClientAutoFacRegistrar(serviceLocator, gameStateMultiplayer);

            serviceLocator.Initialize(clientAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            try
            {
                Client client = serviceLocator.LocateService<Client>();
            }
            catch(Exception ex)
            {
                Assert.Fail($"Caught an exception while resolving Client service: {ex}");
            }

            Assert.Pass();
        }

        public class DummyGameState : GameState
        {
            public DummyGameState() { }
        }
    }
}