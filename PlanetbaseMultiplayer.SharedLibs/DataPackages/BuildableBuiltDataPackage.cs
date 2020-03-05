using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class BuildableBuiltDataPackage : IDataPackage
    {
        public int BuildableId;

        public BuildableBuiltDataPackage(int buildableId)
        {
            BuildableId = buildableId;
        }
    }
}
