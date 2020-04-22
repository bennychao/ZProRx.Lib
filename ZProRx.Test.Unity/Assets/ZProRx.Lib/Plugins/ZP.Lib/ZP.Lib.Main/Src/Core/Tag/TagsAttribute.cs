using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZP.Lib.Main
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple =true, Inherited = true)]
    public class TagsAttribute : Attribute
    {
        public object TagName { get; private set; }

        public string NameSpace { get; private set; }

        public TagsAttribute(string nameSapce,  string tagName)
        {
            this.TagName = tagName;
            this.NameSpace = nameSapce;
        }

        public TagsAttribute(string nameSapce, object tagName)
        {
            this.TagName = tagName;
            this.NameSpace = nameSapce;
        }
    }
}
