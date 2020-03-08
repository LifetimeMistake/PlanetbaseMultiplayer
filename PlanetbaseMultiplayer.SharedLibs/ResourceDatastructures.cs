using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.SharedLibs
{
    [Serializable]
    public class ResourceData
    {
        public int TraderId;
        public Vector3_Serializable Position;
        public Quaternion_Serializable Rotation;
        public Location Location;
        public int SelectableId;
        public Resource.State State;
        public float Durability;
        public float Condition;
    }
    public enum ResourceAction
    {
        Destroy,
        Load,
        Unload,
        AddConstructionMaterial
    }
}
