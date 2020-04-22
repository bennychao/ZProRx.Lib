using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZP.Lib.Core
{
    public class ZRectangle : ZRectangle<float>
    {

    }

    public class ZRectangleSimple : ZRectangle<int>
    {

    }

    public class ZRectangle<TValue> : IZShape
         where TValue : IComparable
        //where TValue 
    {
        public ZProperty<Vector2> Extend = new ZProperty<Vector2>();
        public ZProperty<Vector2> Center = new ZProperty<Vector2>();

        public bool Contain(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        public Vector2 RandomPos()
        {
            throw new NotImplementedException();
        }
    }
}
