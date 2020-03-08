using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class RecycleSelectableDataPackage : IDataPackage
    {
        public int SelectableId;

        public RecycleSelectableDataPackage(int selectableId)
        {
            SelectableId = selectableId;
        }
    }
}
