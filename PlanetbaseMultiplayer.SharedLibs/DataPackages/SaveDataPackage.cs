using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class SaveDataPackage : IDataPackage
    {
        public string XmlData;

        public SaveDataPackage(string xmlData)
        {
            XmlData = xmlData;
        }
    }
}
