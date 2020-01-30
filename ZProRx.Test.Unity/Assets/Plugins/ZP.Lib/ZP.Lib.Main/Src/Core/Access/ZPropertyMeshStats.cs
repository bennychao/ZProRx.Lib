using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ZP.Lib
{
    public static partial class ZPropertyMesh
    {
        static public double Sum<TSource>(object obj, string multiPropID, Func<TSource, double> seletctor)
        {
            var supList = ZPropertyMesh.GetPropertiesEx<TSource>(obj, multiPropID);
            return supList.ToList().Sum( p =>  seletctor(p.Value));
        }

        static public double Sum(object obj, string multiPropID, Func<object, double> seletctor)
        {
            var supList = ZPropertyMesh.GetPropertiesEx(obj, multiPropID);
            return supList.ToList().Sum(p => seletctor(p.Value));
        }

        static public double Sum(object obj, string multiPropID)
        {
            var supList = ZPropertyMesh.GetPropertiesEx(obj, multiPropID);

            return supList.ToList().Sum(p => (float)p.Value);
        }
    }

}
