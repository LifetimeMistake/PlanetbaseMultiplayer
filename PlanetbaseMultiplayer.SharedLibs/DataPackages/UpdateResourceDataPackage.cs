using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class UpdateResourceDataPackage : IDataPackage
    {
        public Guid ResourceId;
        public ResourceAction Action;
        public ResourceData Data;

        public UpdateResourceDataPackage(Guid resourceId, ResourceAction action, ResourceData data)
        {
            ResourceId = resourceId;
            Action = action;
            Data = data;
        }
    }
}
