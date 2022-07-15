using NUnit.Framework;
using PlanetbaseMultiplayer.Model.Autofac;
using PlanetbaseMultiplayer.Server.Autofac;
using System.Runtime.Serialization;
using System;
using PlanetbaseMultiplayer.Model;

namespace PlanetbaseMultiplayer.Server.Tests
{
    public class AutofacTests
    {
        [Test]
        public void ResolveServerTest()
        {
            ServiceLocator serviceLocator = new ServiceLocator();
            ServerSettings serverSettings = new ServerSettings("-", 0, "-");

            ServerAutoFacRegistrar serverAutoFacRegistrar = new ServerAutoFacRegistrar(serviceLocator, serverSettings);

            serviceLocator.Initialize(serverAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            try
            {
                Server server = serviceLocator.LocateService<Server>();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to resolve Server service: {ex}");
            }
        }

        [Test]
        public void ResolveManagersTest()
        {
            ServiceLocator serviceLocator = new ServiceLocator();
            ServerSettings serverSettings = new ServerSettings("-", 0, "-");

            ServerAutoFacRegistrar serverAutoFacRegistrar = new ServerAutoFacRegistrar(serviceLocator, serverSettings);

            serviceLocator.Initialize(serverAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            Type currentType = null;

            try
            {
                foreach (Type managerType in serviceLocator.GetDerivedServiceTypes<IManager>())
                {
                    currentType = managerType;
                    object service = serviceLocator.LocateService(managerType);
                    Warn.If(service.GetType() != managerType);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to resolve {currentType.FullName}: {ex}");
            }
        }

        [Test]
        public void ManagersCountNotEmpty()
        {
            ServiceLocator serviceLocator = new ServiceLocator();
            ServerSettings serverSettings = new ServerSettings("-", 0, "-");

            ServerAutoFacRegistrar serverAutoFacRegistrar = new ServerAutoFacRegistrar(serviceLocator, serverSettings);

            serviceLocator.Initialize(serverAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            if (serviceLocator.GetDerivedServiceTypes<IManager>().Count == 0)
                Assert.Fail("Current ServiceLocator does not contain any registered managers");
        }
    }
}