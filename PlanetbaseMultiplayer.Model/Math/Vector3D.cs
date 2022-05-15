using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Model.Math
{
    [Serializable]
    public struct Vector3D
    {
        public float x;
        public float y;
        public float z;

        public Vector3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3D(Vector3 v) => new Vector3D(v.x, v.y, v.z);
        public static implicit operator Vector3(Vector3D v) => new Vector3(v.x, v.y, v.z);
    }
}
