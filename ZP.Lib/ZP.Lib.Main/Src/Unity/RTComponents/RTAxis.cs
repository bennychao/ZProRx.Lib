//#if ZP_UNITY_CLIENT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZP.Lib
{
    //not used
    [Obsolete("not used")]
    internal static class RTAxis 
    {
        public static Vector3 ToVector3(this Vector2? vec)
        {
            if (vec == null)
                throw new System.Exception("Vector is null");

            return new Vector3(vec.Value.x, 0, vec.Value.y);
        }

        public static Vector3 ToVector3(this Vector2? vec, Vector2 scale)
        {
            if (vec == null)
                throw new System.Exception("Vector is null");

            return new Vector3(vec.Value.x * scale.x, 0, vec.Value.y * scale.y);
        }

        public static Vector3 ToVector3(this Vector2 vec)
        {
            return new Vector3(vec.x, 0, vec.y);
        }

        public static Vector3 ToVector3(this Vector2 vec, Vector2 scale)
        {
            return new Vector3(vec.x * scale.x, 0, vec.y * scale.y);
        }
    }
}

//#endif