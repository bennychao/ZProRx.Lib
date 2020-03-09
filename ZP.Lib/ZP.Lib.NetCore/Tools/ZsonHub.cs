using System;
using System.Collections.Generic;
using ZP.Lib.Core.Values;
using ZP.Lib.Net;
using ZP.Lib.Web;

namespace ZP.Lib.NetCore
{
    public class ZsonListHub<T> : ZsonResult<ZPropertyListHub<T>>
    {
        public ZsonListHub() : base()
        {
            //TData.Node
        }

        public ZsonListHub(ZNetErrorEnum error) : base(error)
        {
        }

        public static implicit operator ZsonListHub<T>(List<T> ds)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonListHub<T>();  // implicit conversion
            newProp.TData.Node.AddRange(ds);
            return newProp;
        }

        public static implicit operator ZsonListHub<T>(ZNetErrorEnum error)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonListHub<T>(error);  // implicit conversion

            return newProp;
        }
    }

    public class ZsonListHub<T, TError> : ZsonResult<ZPropertyListHub<T>, TError>
    {
        public ZsonListHub() : base()
        {
            //TData.Node
        }

        public ZsonListHub(ZNetErrorEnum error) : base(error)
        {

        }

        public ZsonListHub(TError error) : base(error)
        {
        }

        public static implicit operator ZsonListHub<T, TError>(List<T> ds)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonListHub<T, TError>();  // implicit conversion
            newProp.TData.Node.AddRange(ds);
            return newProp;
        }

        public static implicit operator ZsonListHub<T, TError>(ZNetErrorEnum error)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonListHub<T, TError>(error);  // implicit conversion

            return newProp;
        }

        public static implicit operator ZsonListHub<T, TError>(TError error)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonListHub<T, TError>(error);  // implicit conversion

            return newProp;
        }
    }

    public class ZsonHub<T> : ZsonResult<ZPropertyListHub<T>>
    {
        public ZsonHub(T obj) : base()
        {
            TData.Node.Value = obj;
        }

        public static implicit operator ZsonHub<T>(T d)  // implicit digit to byte conversion operator
        {
            var newProp = new ZsonHub<T>(d);  // implicit conversion

            return newProp;
        }
    }
}
