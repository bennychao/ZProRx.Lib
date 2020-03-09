using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.NetCore.Entity
{
    public static class ZPropertyQuery
    {
        public static Dictionary<string, string> Create(IZProperty zProperty)
        {
            var ret = new Dictionary<string, string>();
            ret.Add(zProperty.Name, zProperty.Value.ToString());
            return ret;
        }

        public static Dictionary<string, string> Create(string name, string value)
        {
            var ret = new Dictionary<string, string>();
            ret.Add(name, value);
            return ret;
        }
    }
}
