using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class BuildableSetEnabledDataPackage : IDataPackage
    {
        public int BuildableId;
        public bool Enabled;

        public BuildableSetEnabledDataPackage(int buildableId, bool enabled)
        {
            BuildableId = buildableId;
            Enabled = enabled;
        }
    }
}
