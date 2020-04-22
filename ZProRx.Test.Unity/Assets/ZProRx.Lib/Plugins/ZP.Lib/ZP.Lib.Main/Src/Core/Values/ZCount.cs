using System;
using ZP.Lib;

namespace ZP.Lib.Core.Values
{
    public class ZCount<TCode>
    {

        public ZProperty<TCode> Code = new ZProperty<TCode>();

        public ZProperty<uint> Count = new ZProperty<uint>();

        public ZCount()
        {
        }
        
    }

    public class ZCount<TCode, TId>
    {

        public ZProperty<TCode> Code = new ZProperty<TCode>();

        public ZProperty<TId> Id = new ZProperty<TId>();

        public ZProperty<int> Count = new ZProperty<int>();

        public ZCount()
        {
        }

    }

    public class ZIdCount : ZCount<uint>
    {
        
    }

    public class ZIdCount<TCode> : ZCount<TCode, uint>
    {

    }

    public class ZCodeIdCount : ZCount<string, uint>
    {

    }

    public class ZCodeIdCount<TCode> : ZCodeIdCount
    {
        static public ZCodeIdCount<TCode> Convert(ZCodeIdCount cur)
        {
            var t = (TCode)Enum.Parse(typeof(TCode), cur.Code, true);
            var ret = ZPropertyMesh.CreateObject<ZCodeIdCount<TCode>>();
            ret.Count = cur.Count;
            ret.Code = cur.Code;
            ret.Id = cur.Id;

            return ret;
        }

        public TCode TypedCode=> (TCode)Enum.Parse(typeof(TCode), this.Code, true);
    }
}
