using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class PlaceComponentDataPackage : IDataPackage
    {
        public int ParentModuleId;
        public Quaternion_Serializable Rotation;
        public Vector3_Serializable Position;
        public string ComponentType;

        public PlaceComponentDataPackage(int parentModuleId, Quaternion_Serializable rotation, Vector3_Serializable position, string componentType)
        {
            ParentModuleId = parentModuleId;
            Rotation = rotation;
            Position = position;
            ComponentType = componentType;
        }
    }
}
