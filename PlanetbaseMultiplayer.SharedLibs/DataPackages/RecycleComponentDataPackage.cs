using PlanetbaseMultiplayer.SharedLibs;
using System;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class RecycleComponentDataPackage : IDataPackage
    {
        public int ComponentId;
        public ResourceConstructionData[] CreatedResources;
        public ResourceDestructionData[] DestroyedResources;

        public RecycleComponentDataPackage(int componentId, ResourceConstructionData[] createdResources, ResourceDestructionData[] destroyedResources)
        {
            ComponentId = componentId;
            CreatedResources = createdResources;
            DestroyedResources = destroyedResources;
        }
    }
}