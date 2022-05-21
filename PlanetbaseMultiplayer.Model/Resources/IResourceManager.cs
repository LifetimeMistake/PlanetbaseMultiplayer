using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Resources
{
    public interface IResourceManager : IManager
    {
        Resource CreateResource(Guid resourceId, Guid? containerId, Guid? storageId, Guid? traderId, Type resourceType, ResourceSubtype subtype, ResourceState state);
        Resource CreateResource(Guid? containerId, Guid? storageId, Guid? traderId, Type resourceType, ResourceSubtype subtype, ResourceState state);
        bool StoreResource(Guid resourceId, Guid moduleId);
        bool LoadResource(Guid resourceId);
        bool DropResource(Guid resourceId, ResourceState newState);
        bool DestroyResource(Guid resourceId);
    }
}
