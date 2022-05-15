using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World
{
    [Serializable]
    public class WorldStateData
    {
        public string XmlData;

        public WorldStateData(string xmlData)
        {
            XmlData = xmlData ?? throw new ArgumentNullException(nameof(xmlData));
        }
    }
}
