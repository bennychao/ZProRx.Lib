using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ZP.Lib.CoreEx.Tools;
#if ZP_SERVER

namespace ZP.Lib
{
    internal sealed class BindComponent
    {
        private ZPropertyList<BindComponentParam> Params = new ZPropertyList<BindComponentParam>();
        private ZProperty<string> InType = new ZProperty<string>();
        private ZProperty<string> AssemblyPath = new ZProperty<string>();
        public Type Type
        {
            get
            {
                if (string.IsNullOrEmpty(AssemblyPath.Value))
                {
                    return Type.GetType(InType.Value);
                }
                else
                {
                    Assembly asmb = AssemblyTools.LoadFrom(AssemblyPath.Value);
                    Type supType = asmb.GetType(InType.Value);

                    return supType;
                }
            }
        }

        public BindComponent()
        {
        }

        public void BindGameObject(Transform transform)
        {
            if (Type == null) Debug.LogError("BindGameObject Type not find" );
            var c = transform.AddComponent(Type);
            if (c != null)
            {
                List<FieldInfo> typeInfo = Type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();

                foreach (var p in Params)
                {
                    var target = typeInfo.Find((FieldInfo info) => string.Compare(info.Name, p.Name.Value, StringComparison.Ordinal) == 0);
                    if (target == null)
                        continue;

                    if (typeof(IBindable).IsAssignableFrom( target.FieldType))
                    {
                        var obj = Activator.CreateInstance(target.FieldType);

                        target.SetValue(c, obj);
                        (obj as IBindable).BindData( p);
                    }
                    else if (target.FieldType == typeof(string))
                    {
                        //only support string
                        target.SetValue(c, p.Data.Value);
                    }
                    else //if (target.FieldType == typeof(Vector3))
                    {
                        //for value type
                        //var obj = Activator.CreateInstance(target.FieldType);
                        var  obj =ZPropertyPrefs.LoadValueFromRawData(target.FieldType, p.RawData.Value);
                        target.SetValue(c, obj);

                    }

                }
            }
        }
    }
}
#endif