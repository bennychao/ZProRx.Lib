using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.Core.Values
{

    public class ZDataHub<T> where T : IZProperty,  new()
    {
        protected T node = new T();

        public TData GetValue<TData>()
        {
            return (TData)node.Value;
        }

        public T Node
        {
            set =>  node.Copy(value);

            get => node;
        }

        static public ZDataHub<T> Create(T data)
        {
            var ret = ZPropertyMesh.CreateObject<ZDataHub<T>>();
            ret.Node = data;

            return ret;
        }


        //static public ZDataHub<T> Create()
        //{
        //    var ret = ZPropertyMesh.CreateObject<ZDataHub<T>>();
        //    //ret.Node = data;

        //    return ret;
        //}
    }

    
}
