using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZP.Lib.Common
{
    public class FaceTo : MonoBehaviour
    {
        public Camera target= null;

        void Start()
        {
            if (target == null)
            {
                target = Camera.main;
            }
        }
        void Update()
        {
            transform.LookAt(target.transform.position);
        }
    }
}
