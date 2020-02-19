using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetbaseMultiplayer.SharedLibs
{
    [Serializable]
    public struct ResourceConstructionData
    {
        public string Type;
        public ResourceSubtype Subtype;
        public Vector3_Serializable Position;
        public Quaternion_Serializable Rotation;
        public Location Location;
        public bool Embedded;

        public ResourceConstructionData(string type, ResourceSubtype subtype, Vector3_Serializable position, Quaternion_Serializable rotation, Location location, bool embedded)
        {
            Type = type;
            Subtype = subtype;
            Position = position;
            Rotation = rotation;
            Location = location;
            Embedded = embedded;
        }
    }

    [Serializable]
    public struct ResourceDestructionData
    {
        public int ResourceId;

        public ResourceDestructionData(int resourceId)
        {
            ResourceId = resourceId;
        }
    }
}
