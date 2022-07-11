using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Autofac
{
    public class ServiceLocator
    {
        private IContainer container;
        private ILifetimeScope lifetimeScope;

        public ServiceLocator(params IAutoFacRegistrar[] registrars)
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();

            foreach(var registrar in registrars)
                registrar.RegisterComponents(containerBuilder);

            container = containerBuilder.Build();
        }

        public void BeginLifetimeScope()
        {
            EndLifetimeScope();
            lifetimeScope = container.BeginLifetimeScope();
        }

        public void EndLifetimeScope()
        {
            lifetimeScope?.Dispose();
        }

        public bool LifetimeScopeExists()
        {
            return lifetimeScope != null;
        }

        private void AssertLifetimeScopeExists()
        {
            if (!LifetimeScopeExists())
                throw new InvalidOperationException("You must create a new lifetime scope before resolving services.");
        }

        public T LocateService<T>() where T : class
        {
            AssertLifetimeScopeExists();
            return container.Resolve<T>();
        }

        public object LocateService(Type serviceType)
        {
            AssertLifetimeScopeExists();
            return container.Resolve(serviceType);
        }

        public List<T> LocateServicesOfType<T>() where T : class
        {
            AssertLifetimeScopeExists();
            object obj = container.Resolve<T>();
            return null;
        }
    }
}
