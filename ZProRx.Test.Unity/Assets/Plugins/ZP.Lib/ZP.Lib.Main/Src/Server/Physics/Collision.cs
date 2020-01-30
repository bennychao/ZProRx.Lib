using System;
#if ZP_SERVER
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    public class Collision
    {
        public Collision()
        {
        }

        public Collider collider
        {
            internal set;
            get;
        }

        public GameObject gameObject => collider.gameObject;

        public Transform transform => collider.transform;

        public Collider other
        {
            internal set;
            get;
        }

        public List<ContactPoint> contacts
        {
            get;
        }
    }
}
#endif