using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZP.Lib.Core
{

    public class ZCircle : ZCircle<float>
    {

    }

    public class ZCircleSimple : ZCircle<int>
    {

    }

    public class ZCircle<TValue> : IZShape
        where TValue : IComparable
        //where TValue 
    {
        public ZProperty<TValue> Radius = new ZProperty<TValue>();

        public ZProperty<Vector2> Center = new ZProperty<Vector2>();

        public bool Contain(Vector2 pos)
        {
            var dis = Vector2.Distance(pos, Center);

            return Radius.Value.CompareTo(dis) >= 1;
        }

        public Vector2 RandomPos()
        {
            var r = RandomMgr.Next(0, (float)(object)Radius.Value);

            var dir = new Vector2( RandomMgr.Next(-1.0f, 1.0f), RandomMgr.Next(-1.0f, 1.0f));
            dir.Normalize();

            var ret = Center + (dir * r);

            return ret;
        }
    }
}
