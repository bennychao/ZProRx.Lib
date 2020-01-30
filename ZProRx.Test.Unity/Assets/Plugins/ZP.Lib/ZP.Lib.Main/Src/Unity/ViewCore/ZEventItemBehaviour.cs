using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ZP.Lib
{
    public class ZEventItemBehaviour<T> : ZEventItemBehaviour
    {
        protected IZEvent<T> Event { get => zEvent as IZEvent<T>; }
    }


    public class ZEventItemBehaviour : MonoBehaviour
    {
        protected IZEvent zEvent;
        protected void BindBase(IZEvent e)
        {
            zEvent = e;
        }
    }
}

