using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class RecycleSelectableDataPackage : IDataPackage
    {
        public int SelectableId;
        public ResourceConstructionData[] CreatedResources;

        public RecycleSelectableDataPackage(int selectableId, ResourceConstructionData[] createdResources)
        {
            SelectableId = selectableId;
            CreatedResources = createdResources;
        }
    }
}
