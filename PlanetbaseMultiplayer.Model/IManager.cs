using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model
{
    public interface IManager
    {
        bool IsInitialized { get; }
        void Initialize();
    }
}
