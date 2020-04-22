using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZP.Lib.Common;

namespace ZP.Lib
{
    /// <summary>
    /// return list<object> Dictionary<string, object> object null valueType
    /// </summary>
    public class ZPropertyMapper
    {

        static public Dictionary<string, object> ConvertToMap(object obj)
        {
            if (ZPropertyMesh.IsNullValue(obj))
            {
                return null;
            }
            //else if (ZPropertyMesh.IsRawDataRef(obj.GetType()))
            //{
            //    return (obj as IRawDataPref)?.ToValue();
            //}
            else if (ZPropertyMesh.IsPropertable(obj))
            {
                return _ConvertToMap(obj);
            }
            else if (ZPropertyMesh.IsProperty(obj))
            {
                return _ConvertToMap((obj as IZProperty).Value);
            }
            else if (obj is Vector2)
            {
                return ConvertVector2((Vector2)obj);
            }            
            else if (obj is Quaternion)
            {
                return ConvertQuaternion((Quaternion)obj);
            }
            else if (ZPropertyMesh.IsPropertyList(obj as IZProperty))
            {

            }


            return null;
        }

        static public object ConvertToValue(object v)
        {
            if (v is float)
            {
                float newP = (float)(v);
                return ((double)newP);
            }
            else if (v.GetType().IsEnum)
            {
               return v.ToString();
            }
            else if (ZPropertyMesh.IsMultiEnum(v.GetType()))
            {
                return v.ToString();
            }
            else if (ZPropertyMesh.IsNullValue(v))
            {
               return null;
            }
            return v;
        }

        static public List<object> ConvertToList(object obj)
        {
            List<object> rets = new List<object>();
            if (ZPropertyMesh.IsPropertyList(obj as IZProperty))
            {
                var list = (obj as IZPropertyList).PropList.Select(a => a.Value).ToList();

                foreach (var v in list)
                {

                    if (ZPropertyMesh.IsValueType(v.GetType()))
                    {
                        rets.Add(ConvertToValue(v));
                    }
                    else
                    {
                        rets.Add(ConvertToMap(v));
                    }

                }
            }


            return rets;
        }

        /// <summary>
        /// Converts to map.
        /// </summary>
        static private Dictionary<string, object> _ConvertToMap(object obj)
        {
            //all other value type property will net convert

            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);
            //if (ZPropertyMesh.IsRawDataRef(obj.GetType()))
            //{
            //    return (obj as IRawDataPref)?.ToValue();
            //}


            Dictionary<string, object> ret = new Dictionary<string, object>();
            foreach (var p in props)
            {
                if (ZPropertyMesh.IsRef(p))
                {
                    ret[p.PropertyID] = (p as IRefable).ToMap();
                    continue;
                }

                if (p.Value == null && !ZPropertyMesh.IsPropertyList(p))
                    continue;

                if (ZPropertyMesh.IsRawDataRef(p.GetDefineType()))
                {
                    //ret[p.PropertyID] =( (p.Value as IRawDataPref)?.RawData as JsonData)?.ToJson();
                    ret[p.PropertyID] = (p.Value as IRawDataPref)?.ToValue();
                    continue;
                }

                if (ZPropertyMesh.IsDefineInterface(p) || ZPropertyMesh.IsPropertyRefList(p))
                    continue;

                if (ZPropertyMesh.IsPropertyList(p))
                {
                    var list = ConvertToList(p);//(p as IZPropertyList).ConvertToArray();
                    if (list?.Count > 0)
                        ret[p.PropertyID] = list;
                }
                else if (ZPropertyMesh.IsPropertable(p.Value.GetType()))
                {
                    ret[p.PropertyID] = ConvertToMap(p.Value);
                }
                else if (ZPropertyMesh.IsRef(p))
                {
                    ret[p.PropertyID] = (p as IRefable).ToMap();
                }
                else if (p.Value is float)
                {
                    float newP = (float)(p.Value);
                    ret[p.PropertyID] = (double)newP;
                }
                else if (p.Value.GetType().IsEnum)
                {
                    ret[p.PropertyID] = p.Value.ToString();
                }
                else if (ZPropertyMesh.IsMultiEnum(p))
                {
                    ret[p.PropertyID] = (p.Value as IMultiEnumerable).ToString();
                }
                else if (ZPropertyMesh.IsNullValue(p))
                {
                    ret[p.PropertyID] = null;
                }
                else if (p.Value is Vector2)
                {
                    ret[p.PropertyID] = ConvertVector2((Vector2)(p.Value));
                }
                else if (p.Value is Vector3)
                {
                    ret[p.PropertyID] = ConvertVector3((Vector3)(p.Value));
                }
                else if (p.Value is Quaternion)
                {
                    ret[p.PropertyID] = ConvertQuaternion((Quaternion)(p.Value));
                }
                else
                {
                    ret[p.PropertyID] = p.Value;
                }
            }

            return ret;
        }

        static public Dictionary<string, object> ConvertToMap(Vector2 vec)
        {
            return ConvertVector2(vec);
        }


        static public Dictionary<string, object> ConvertVector2(Vector2 vec)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            ret["x"] = (double)vec.x;
            ret["y"] = (double)vec.y;

            return ret;
        }

        static public Dictionary<string, object> ConvertQuaternion(Quaternion rot)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            ret["x"] = (double)rot.x;
            ret["y"] = (double)rot.y;
            ret["z"] = (double)rot.z;
            ret["w"] = (double)rot.w;
            return ret;
        }


        static public Dictionary<string, object> ConvertVector3(Vector3 vec)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            ret["x"] = (double)vec.x;
            ret["y"] = (double)vec.y;
            ret["z"] = (double)vec.z;

            return ret;
        }

        //static public Dictionary<string, object> ConvertRawData(IRawDataPref rawData)
        //{
        //    return _ConvertToMap(rawData.RawData);
        //}
        //static public string ConvertEnum(object obj)
        //{
        //    return obj.ToString();
        //}

    }
}


