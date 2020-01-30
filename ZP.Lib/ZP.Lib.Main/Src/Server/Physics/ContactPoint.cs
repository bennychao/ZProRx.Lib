using System;
using UnityEngine;

namespace UnityEngine
{
#if ZP_SERVER
    public class ContactPoint
    {
        public ContactPoint()
        {
        }

        public Vector3 point
        {
            set;
            get;
        }
        public Vector3 normal
        {
            set;
            get;
        }
    }
#endif

}
