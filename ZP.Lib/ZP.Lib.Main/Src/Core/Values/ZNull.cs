using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    public struct ZNull : IEquatable<ZNull>
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

        public static bool operator ==(ZNull first, ZNull second)
        {
            return true;
        }

        public static bool operator !=(ZNull first, ZNull second)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is ZNull;
        }


        public bool Equals(ZNull other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "(ZNull)";
        }

        //public static bool operator ==(ZNull e1, ZNull e2)
        //{
        //    return (int)e1.value == (int)(object)e2;
        //}

        //public static bool operator != (ZNull e1, ZNull e2)
        //{
        //    return (int)e1.value != Convert.ToInt32(e2);// (int)(object)e2;
        //}
    }

}

