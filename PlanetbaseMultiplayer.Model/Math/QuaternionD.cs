using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlanetbaseMultiplayer.Model.Math
{
    [Serializable]
    public struct QuaternionD
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public QuaternionD(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator QuaternionD(Quaternion v) => new QuaternionD(v.x, v.y, v.z, v.w);
        public static implicit operator Quaternion(QuaternionD v) => new Quaternion(v.x, v.y, v.z, v.w);
    }
}
