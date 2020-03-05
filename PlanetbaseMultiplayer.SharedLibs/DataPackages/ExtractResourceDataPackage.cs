using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class ExtractResourceDataPackage : IDataPackage
    {
        public int ResourceId;

        public ExtractResourceDataPackage(int resourceId)
        {
            ResourceId = resourceId;
        }
    }
}
