using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Autofac
{
    public interface IAutoFacRegistrar
    {
        void RegisterComponents(ContainerBuilder builder);
    }
}
