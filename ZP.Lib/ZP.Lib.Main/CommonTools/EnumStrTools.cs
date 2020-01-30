using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZP.Lib.Common
{
    public static class EnumStrTools
    {
        public static string GetStr<T>(T value)// where T : Enum
        {
            var mem = typeof(T).GetMember(value.ToString());

            if (mem != null && mem.Count() <= 0)
                return value.ToString();

            var attrs = mem[0].GetCustomAttributes(typeof(EnumStrAttribute), true);
            if (attrs != null && attrs.Count() <= 0)
                return value.ToString();

            return  (attrs[0] as EnumStrAttribute)?.Name;
        }
    }
}
