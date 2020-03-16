using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class UpdateBuildableDataPackage : IDataPackage
    {
        public Guid BuildableId;
        public BuildableAction Action;
        public BuildableData Data;

        public UpdateBuildableDataPackage(Guid buildableId, BuildableAction action, BuildableData data)
        {
            BuildableId = buildableId;
            Action = action;
            Data = data;
        }
    }
}
