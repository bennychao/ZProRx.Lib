using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    public struct ZNull 
    {
        static readonly ZNull defaultNull = new ZNull();// = Default(ZNull);
        public static ZNull Default => defaultNull;

        public static bool IsNull(Type t)
        {
            return t == typeof(ZNull);
        }

        public static bool IsNull(object o)
        {
            return IsNull(o.GetType());
        }
    }

}

