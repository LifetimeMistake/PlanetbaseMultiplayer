using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlanetbaseMultiplayer.SharedLibs.DataPackages
{
    [Serializable]
    public class PlaceModuleDataPackage : IDataPackage
    {
        public Vector3_Serializable Position;
        public int SizeIndex;
        public string ModuleType;

        public PlaceModuleDataPackage(Vector3_Serializable position, int sizeIndex, string moduleType)
        {
            Position = position;
            SizeIndex = sizeIndex;
            ModuleType = moduleType;
        }

        public PlaceModuleDataPackage(Vector3 position, int sizeIndex, string moduleType)
        {
            Position = (Vector3_Serializable)position;
            SizeIndex = sizeIndex;
            ModuleType = moduleType;
        }
    }
}
