using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class RecycleColonyShipDataPackage : IDataPackage
    {
        public int ColonyShipId;
        public ResourceUpdateData[] ExtractedResources;
        public ResourceConstructionData[] CreatedResources;

        public RecycleColonyShipDataPackage(int colonyShipId, ResourceUpdateData[] extractedResources, ResourceConstructionData[] createdResources)
        {
            ColonyShipId = colonyShipId;
            ExtractedResources = extractedResources;
            CreatedResources = createdResources;
        }
    }
}
