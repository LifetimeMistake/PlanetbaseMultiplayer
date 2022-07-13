using Planetbase;
using NUnit.Framework;
using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Server.Autofac;
using System.Runtime.Serialization;

namespace PlanetbaseMultiplayer.Server.Tests
{
    public class AutofacTests
    {
        [Test]
        public void ResolveServerTest()
        {
            ServerSettings serverSettings = new ServerSettings("-", 0, "-");
            ServiceLocator serviceLocator = new ServiceLocator();

            ServerAutoFacRegistrar serverAutoFacRegistrar = new ServerAutoFacRegistrar(serviceLocator, serverSettings);

            serviceLocator.Initialize(serverAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            try
            {
                Server server = serviceLocator.LocateService<Server>();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Caught an exception while resolving Server service: {ex}");
            }

            Assert.Pass();
        }
    }
}