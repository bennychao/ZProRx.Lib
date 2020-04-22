using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZP.Lib.Main;

namespace ZP.Lib
{
    public static partial class ZPropertyMesh
    {
        static  public string GetTag(IZProperty property, string nameSpace)
        {
            var tagAttrs =  ZPropertyAttributeTools.GetAttributes<TagsAttribute>(property);

            var tagAttr = tagAttrs.Find(ta => string.Compare(ta.NameSpace, nameSpace) == 0);

            return tagAttr?.TagName.ToString();
        }

        static public List<(string ns, string tag)> GetTags(IZProperty property)
        {
            var tagAttrs = ZPropertyAttributeTools.GetAttributes<TagsAttribute>(property);

            return tagAttrs.Select(ta => (ns : ta.NameSpace, tag: ta.TagName.ToString())).ToList();
        }

        static public string GetTag(IZEvent ev, string nameSpace)
        {
            var tagAttrs = ev.AttributeNode.GetAttributes<TagsAttribute>();

            var tagAttr = tagAttrs.Find(ta => string.Compare(ta.NameSpace, nameSpace) == 0);

            return tagAttr?.TagName.ToString();
        }

        static public TTag GetTag<TTag>(IZEvent ev, string nameSpace)
        {
            var tagAttrs = ev.AttributeNode.GetAttributes<TagsAttribute>();

            var tagAttr = tagAttrs.Find(ta => string.Compare(ta.NameSpace, nameSpace) == 0);

            return (TTag)(tagAttr?.TagName);
        }

        static public List<(string ns, string tag)> GetTags(IZEvent ev)
        {
            var tagAttrs = ev.AttributeNode.GetAttributes<TagsAttribute>();

            return tagAttrs.Select(ta => (ns: ta.NameSpace, tag: ta.TagName.ToString())).ToList();
        }
    }
}
