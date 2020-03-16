using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class AddBuildableDataPackage : IDataPackage
    {
        public Guid BuildableId;
        public string XmlData;

        public AddBuildableDataPackage(Guid buildableId, string xmlData)
        {
            BuildableId = buildableId;
            XmlData = xmlData;
        }
    }
}
