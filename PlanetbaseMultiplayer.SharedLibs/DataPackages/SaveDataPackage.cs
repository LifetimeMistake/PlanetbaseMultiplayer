using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class SaveDataPackage : IDataPackage
    {
        public string XmlData;
        public int NextId;
        public int NextBotId;

        public SaveDataPackage(string xmlData, int nextId, int nextBotId)
        {
            XmlData = xmlData;
            NextId = nextId;
            NextBotId = nextBotId;
        }
    }
}
