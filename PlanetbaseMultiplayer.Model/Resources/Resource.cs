using PlanetbaseMultiplayer.Model.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Resources
{
    [Serializable]
    public struct Resource
    {
        public Guid Id;
        public Guid? ContainerId; // Component container Id
        public Guid? StorageId; // Storage building Id
        public Guid? TraderId; // Trader ship Id
        public Type Type;
        public ResourceSubtype Subtype;
        public ResourceState State;

        public Vector3D Position;
        public QuaternionD Rotation;

        public Resource(Guid id, Guid? containerId, Guid? storageId, Guid? traderId, Type type, ResourceSubtype subtype, ResourceState state)
        {
            Id = id;
            ContainerId = containerId;
            StorageId = storageId;
            TraderId = traderId;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Subtype = subtype;
            State = state;
            Position = new Vector3D();
            Rotation = new QuaternionD();
        }

        public Resource(Guid id, Guid? containerId, Guid? storageId, Guid? traderId, Type type, ResourceSubtype subtype, ResourceState state, Vector3D position, QuaternionD rotation)
        {
            Id = id;
            ContainerId = containerId;
            StorageId = storageId;
            TraderId = traderId;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Subtype = subtype;
            State = state;
            Position = position;
            Rotation = rotation;
        }
    }
}
