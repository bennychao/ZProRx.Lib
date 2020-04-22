using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZP.Lib.Common
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumStrAttribute: Attribute
    {
        public string Name
        {
            get;set;
        }

        public EnumStrAttribute(string name)
        {
            Name = name;
        }
    }
}
