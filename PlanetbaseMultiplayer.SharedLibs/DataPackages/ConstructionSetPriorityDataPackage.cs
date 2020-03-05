using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class ConstructionSetPriorityDataPackage : IDataPackage
    {
        public int ConstructionId;
        public bool HighPriority;

        public ConstructionSetPriorityDataPackage(int constructionId, bool highPriority)
        {
            ConstructionId = constructionId;
            HighPriority = highPriority;
        }
    }
}
