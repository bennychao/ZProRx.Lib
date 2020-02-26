using System;
using System.Linq;
using ZP.Lib;
using ZP.Lib.Card.Entity;

namespace ZP.Lib.Card.Tools
{
    public class ZMaterialCollection
    {
        IZPropertyList<ZMaterial> propertyList = null;

        public ZMaterialCollection()
        {
        }

        public void Bind(object obj)
        {
            propertyList = ZPropertyMesh.GetProperties(obj, typeof(ZMaterial))
                .Select(a => a as IZPropertyList<ZMaterial>)
                ?.FirstOrDefault();


        }

        public ZMaterial GetMaterial(string mateTypeStr)
        {
            //var linkTypeStr = cardTypeStr + "Link";
            return propertyList.PropList.Find(p => p.GetDefineType().Name.Contains(mateTypeStr))?.Value as ZMaterial;

            //return default(TCardLink);
        }
    }
}
