using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs
{
    [Serializable]
    public class BuildableData
    {
        public bool Enabled;
        public bool HighPriority;
        public BuildableState State;
    }
    public enum BuildableAction
    {
        Destroy,
        SetEnabled,
        SetPriority
    }
}
