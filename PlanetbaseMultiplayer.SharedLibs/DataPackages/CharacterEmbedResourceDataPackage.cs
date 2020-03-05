using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class CharacterEmbedResourceDataPackage : IDataPackage
    {
        public int CharacterId;
        public int ComponentId;
        public Resource.State ResourceState;

        public CharacterEmbedResourceDataPackage(int characterId, int componentId, Resource.State resourceState)
        {
            CharacterId = characterId;
            ComponentId = componentId;
            ResourceState = resourceState;
        }
    }
}
