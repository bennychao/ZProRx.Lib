using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZP.Lib.Common
{

    public interface IMultiEnumerable
    {
        object Parse(string str);
        uint UintValue { get; }

        string ToString();
    }

    /// <summary>
    /// TE1 and TE2 must be continuous int/uint
    /// </summary>
    /// <typeparam name="TE1"></typeparam>
    /// <typeparam name="TE2"></typeparam>
    public struct MultiEnum<TE1, TE2>  : IMultiEnumerable, IComparable, IConvertible
    //where TE1: Enum 
    //where TE2: Enum
    {

        //private List<>
        private uint value;

        public uint UintValue => value;

        private static uint ParseMax(Type enumType)
        {
            var last = enumType.GetEnumNames().ToList()?.Last();
            return (uint)(int)Enum.Parse(enumType, last, true);
        }

        public override string ToString()
        {
            var maxE1 = ParseMax(typeof(TE1));

            if (value < maxE1)
                return ((TE1)(object)(int)(value)).ToString();

            var maxE2 = ParseMax(typeof(TE2));

            if (value < maxE2)
                return ((TE2)(object)(int)(value)).ToString();

            throw new Exception("value is not MultiEnum");
        }

        public object Parse(string str)
        {
            MultiEnum<TE1, TE2> cur  = (MultiEnum<TE1, TE2>)(str);
            this.value = cur.value;
            return this;
        }
        static public MultiEnum<TE1, TE2> ParseStr(string str)
        {
            MultiEnum<TE1, TE2> cur = (MultiEnum<TE1, TE2>)(str);
            
            return cur;
        }


        public int CompareTo(object obj)
        {
            var target = Convert.ToInt32(obj);
            return (int)this - target; //(int)(MultiEnum<TE1, TE2>)obj;
        }

        public static implicit operator TE1(MultiEnum<TE1, TE2> e)  // implicit digit to byte conversion operator
        {
            var maxE1 = ParseMax(typeof(TE1));
            if (e.value < maxE1)
                return (TE1)(object)(int)(e.value);

            throw new Exception("value is not MultiEnum");
        }

        public static implicit operator TE2(MultiEnum<TE1, TE2> e)  // implicit digit to byte conversion operator
        {
            var maxE2 = ParseMax(typeof(TE2));
            if (e.value < maxE2)
                return (TE2)(object)(int)(e.value);

            throw new Exception("value is not MultiEnum");
        }

        public static implicit operator int(MultiEnum<TE1, TE2> e)  // implicit digit to byte conversion operator
        {
            return (int)e.value;
        }

        public static implicit operator uint(MultiEnum<TE1, TE2> e)  // implicit digit to byte conversion operator
        {
            return e.value;
        }

        public static implicit operator MultiEnum<TE1, TE2>(TE1 e)  // implicit digit to byte conversion operator
        {
            MultiEnum<TE1, TE2> ret = new MultiEnum<TE1, TE2>();

            ret.value = Convert.ToUInt32(e); //(int)(object)e;

            return ret;
        }

        public static implicit operator MultiEnum<TE1, TE2>(TE2 e)  // implicit digit to byte conversion operator
        {
            MultiEnum<TE1, TE2> ret = new MultiEnum<TE1, TE2>();

            ret.value = Convert.ToUInt32(e);//(uint)(int)(object)e;

            return ret;
        }

        public static implicit operator MultiEnum<TE1, TE2>(string strValue)  // implicit digit to byte conversion operator
        {
            MultiEnum<TE1, TE2> ret = new MultiEnum<TE1, TE2>();
            try
            {
                var obj = Enum.Parse(typeof(TE1), strValue, true);
                ret.value = (uint)(int)obj;
            }
            catch
            {
                try
                {
                    ret.value = (uint)(int)Enum.Parse(typeof(TE2), strValue, true);
                }
                catch
                {
                    throw new Exception("value is not MultiEnum");
                }
            }

            return ret;
        }

        public static bool operator ==(MultiEnum<TE1, TE2> e1, TE1 e2)
        {
            return (int)e1.value == (int)(object)e2;
        }




        public static bool operator !=(MultiEnum<TE1, TE2> e1, TE1 e2)
        {
            return (int)e1.value != Convert.ToInt32(e2);// (int)(object)e2;
        }

        public static bool operator ==(TE1 e2, MultiEnum<TE1, TE2> e1)
        {
            return (int)e1.value == Convert.ToInt32(e2);// (int)(object)e2;
        }

        public static bool operator !=(TE1 e2, MultiEnum<TE1, TE2> e1)
        {
            return (int)e1.value != Convert.ToInt32(e2);// (int)(object)e2;
        }

        public static bool operator ==(MultiEnum<TE1, TE2> e1, TE2 e2)
        {
            return (int)e1.value == Convert.ToInt32(e2);// (int)(object)e2;
        }

        public static bool operator !=(MultiEnum<TE1, TE2> e1, TE2 e2)
        {
            return (int)e1.value != Convert.ToInt32(e2);// (int)(object)e2;
        }

        public static bool operator ==(TE2 e2, MultiEnum<TE1, TE2> e1)
        {
            return (int)e1.value == Convert.ToInt32(e2);// (int)(object)e2;
        }

        public static bool operator !=(TE2 e2, MultiEnum<TE1, TE2> e1)
        {
            return (int)e1.value != Convert.ToInt32(e2);// (int)(object)e2;
        }

        public static List<object> GetValues()
        {
            List<object> ret = new List<object>();

            foreach (var o in Enum.GetValues(typeof(TE1)))
            {
                ret.Add(o);
            }
            foreach (var o in Enum.GetValues(typeof(TE2)))
            {
                ret.Add(o);
            }


            return ret;
        }

        public override Boolean Equals(Object obj)
        {
            return (int)((MultiEnum<TE1, TE2>)obj).value == (int)value;
        }

        public override int GetHashCode()
         {
             return (int)value;
         }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int32;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (short)value;
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (int)value;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return (long)value;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return (object)value;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort)value;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (uint)value;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return (ulong)value;
        }
    }
}
