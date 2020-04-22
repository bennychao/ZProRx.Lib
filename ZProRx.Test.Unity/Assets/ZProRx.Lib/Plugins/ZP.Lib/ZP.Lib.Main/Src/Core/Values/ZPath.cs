using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZP.Lib
{

    public class PathNode2<T> : IIndexable
    {
        public ZProperty<short> ID;

        public ZProperty<ZTransform> Trans;

        public ZProperty<T> Data;

        public int Index { get => ID.Value; set => ID.Value = (short)value; }

        public ZPropertyList<short> Links = new ZPropertyList<short>();

    }

    public class ZPath2<T> 
    {
        public ZPropertyList<PathNode2<T>> Path = new ZPropertyList<PathNode2<T>>();

        public ZProperty<short> CurNode = new ZProperty<short>();
        public void Go(Vector2 pos)
        {

        }

        public void Go(PathNode2<T> node)
        {

        }

        public bool BindData(short id, T data)
        {
            var p = Path.FindValue(a => a.ID == id);
            if (p != null)
            {
                p.Data.Value = data;
                return true;
            }

            return false;
        }

        public void Go(T value)
        {
            var v = Path.FindValue((arg) => (object)arg.Data.Value == (object)value);
            CurNode.Value = v?.ID;
        }
    }
}

