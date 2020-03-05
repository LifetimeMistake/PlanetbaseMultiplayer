using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class CharacterDestroyResourceDataPackage : IDataPackage
    {
        public int CharacterId;
        public CharacterDestroyResourceDataPackage(int characterId)
        {
            CharacterId = characterId;
        }
    }
}
