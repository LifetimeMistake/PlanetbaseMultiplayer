using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class AddConstructionMaterialDataPackage : IDataPackage
    {
        public int BuildableId;
        public int ResourceId;

        public AddConstructionMaterialDataPackage(int buildableId, int resourceId)
        {
            BuildableId = buildableId;
            ResourceId = resourceId;
        }
    }
}
