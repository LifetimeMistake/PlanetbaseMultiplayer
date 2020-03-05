using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class CharacterLoadResourceDataPackage : IDataPackage
    {
        public int CharacterId;
        public int ResourceId;

        public CharacterLoadResourceDataPackage(int characterId, int resourceId)
        {
            CharacterId = characterId;
            ResourceId = resourceId;
        }
    }
}
