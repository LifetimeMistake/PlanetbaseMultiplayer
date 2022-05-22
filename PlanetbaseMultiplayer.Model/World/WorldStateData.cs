using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.World
{
    [Serializable]
    public struct WorldStateData
    {
        private string xmlData;

        public string XmlData { get => xmlData; set => xmlData = value; }

        public WorldStateData(string xmlData)
        {
            this.xmlData = xmlData ?? throw new ArgumentNullException(nameof(xmlData));
        }
    }
}
