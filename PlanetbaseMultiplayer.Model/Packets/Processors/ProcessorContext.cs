using PlanetbaseMultiplayer.Model.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Packets.Processors
{
    // Used to share the service locator structure with the processor
    public class ProcessorContext
    {
        public ServiceLocator ServiceLocator;

        public ProcessorContext(ServiceLocator serviceLocator)
        {
            ServiceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));
        }
    }
}
