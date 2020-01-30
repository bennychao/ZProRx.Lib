

using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Core.Values
{
    public class ZPropertyListHub<TData> : ZDataHub<ZPropertyList<TData>>
    {
        public static ZPropertyListHub<TData> CreateList(List<TData> datas)
        {
            var hub = ZPropertyMesh.CreateObject<ZPropertyListHub<TData>>();
            hub.Node.AddRange(datas);
            return hub;
        }
    }
}


