using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class CharacterStoreResourceDataPackage : IDataPackage
    {
        public int CharacterId;
        public int ModuleId;

        public CharacterStoreResourceDataPackage(int characterId, int moduleId)
        {
            CharacterId = characterId;
            ModuleId = moduleId;
        }
    }
}
