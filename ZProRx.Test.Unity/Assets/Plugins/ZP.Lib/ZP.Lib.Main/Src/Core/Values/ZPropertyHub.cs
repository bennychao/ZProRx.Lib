using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Core.Values;

namespace ZP.Lib.Core.Values
{
    public class ZPropertyHub<TData> : ZDataHub<ZProperty<TData>>
    {
        public TData Value
        {
            get => Node.Value;

            set => Node.Value = value;
        }

        static public ZPropertyHub<TData> CreateHub(object param)
        {
            var ret = ZPropertyMesh.CreateObject<ZPropertyHub<TData>>();

            var data = ZPropertyMesh.CreateObjectWithParam<TData>(param);

            ret.node.Value = data;

            return ret;
        }
    }


}
