using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class CharacterUnloadResourceDataPackage : IDataPackage
    {
        public int CharacterId;
        public Resource.State ResourceState;

        public CharacterUnloadResourceDataPackage(int characterId, Resource.State resourceState)
        {
            CharacterId = characterId;
            ResourceState = resourceState;
        }
    }
}
