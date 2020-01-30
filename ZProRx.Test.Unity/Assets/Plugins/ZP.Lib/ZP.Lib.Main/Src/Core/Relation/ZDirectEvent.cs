using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Core.Domain;

namespace ZP.Lib.Core.Relation
{
    public class ZDirectEvent : ZEvent, IDirectLinkable
    {
        protected object parentObj = null;
        public object Parent => parentObj;

        public void InitParent(object obj)
        {
            parentObj = obj;
        }
    }

    public class ZDirectEvent<T> : ZEvent<T>, IDirectLinkable
    {
        protected object parentObj = null;
        public object Parent => parentObj;

        public void InitParent(object obj)
        {
            parentObj = obj;
        }
    }
}
