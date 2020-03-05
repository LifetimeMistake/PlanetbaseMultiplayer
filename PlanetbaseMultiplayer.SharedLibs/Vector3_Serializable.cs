using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PlanetbaseMultiplayer.SharedLibs
{
    [Serializable]
    public struct Vector3_Serializable
    {
        public float x;
        public float y;
        public float z;

        public Vector3_Serializable(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3_Serializable(Vector3 v) => new Vector3_Serializable(v.x, v.y, v.z);
        public static implicit operator Vector3(Vector3_Serializable v) => new Vector3(v.x, v.y, v.z);
    }
}
