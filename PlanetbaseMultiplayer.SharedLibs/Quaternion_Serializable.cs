using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.SharedLibs
{
    [Serializable]
    public struct Quaternion_Serializable
    {
        public float x;
        public float y;
        public float z;
        public float w;

        private bool is_initialized;
        public bool IsEmpty { get { return !is_initialized; } }
        public Quaternion_Serializable(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            is_initialized = true;
        }

        public static explicit operator Quaternion_Serializable(Quaternion v) => new Quaternion_Serializable(v.x, v.y, v.z, v.w);
        public static explicit operator Quaternion(Quaternion_Serializable v) => new Quaternion(v.x, v.y, v.z, v.w);
    }
}
