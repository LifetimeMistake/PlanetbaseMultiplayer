using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class AddResourceDataPackage : IDataPackage
    {
        public Guid ResourceId;
        public string ResourceType;
        public string XmlData;

        public AddResourceDataPackage(Guid resourceId, string resourceType, string xmlData)
        {
            ResourceId = resourceId;
            ResourceType = resourceType;
            XmlData = xmlData;
        }
    }
}
