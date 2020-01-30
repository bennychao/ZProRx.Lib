using System;
using System.Linq;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.Collections;
using System.IO;
using UniRx;
using UnityEngine.Assertions;
using ZP.Lib.Core;
using ZP.Lib.Common;
using System.Reflection;
using ZP.Lib.CoreEx.Tools;

namespace ZP.Lib
{

    /// <summary>
    /// Z property json lit prefs.
    /// </summary>
    public class ZPropertyJsonLitPrefs : IZPropertyPrefs
    {
        string strTestData = "";
        public void Save(object obj, string path)
        {
            var map = ZPropertyMapper.ConvertToMap(obj);

            //			Dictionary<string, int> dic = new Dictionary<string, int> ();
            //			dic ["3123"] = 321;
            strTestData = JsonMapper.ToJson(map);

            //Debug.Log("strData " + strTestData);
            File.WriteAllText(path, strTestData);
        }

        /// <summary>
        /// Load the specified obj and path.
        /// </summary>
        public void Load(object obj, string path)
        {
            string strData = File.ReadAllText(path);

            LoadFromStr(obj, strData);
        }


        public void LoadFromStr(object obj, string strData)
        {
            JsonData data = JsonMapper.ToObject(strData);
            if (data.Count <= 0)
                throw new Exception("JsonReadError");
            //Debug.Log ("data is " + data ["Weapon.power"]);

            ConvertToObject(obj, data);
        }

        public void LoadValueFromStr(IZProperty property, string strData)
        {
            JsonData data = JsonMapper.ToObject(strData);
            if (data.Count <= 0)
                throw new Exception("JsonReadError");
            //Debug.Log ("data is " + data ["Weapon.power"]);

            ConvertProperty(property, data);
        }


        public void ConvertToObject(object obj, JsonData data) {
            List<IZProperty> props = ZPropertyMesh.GetProperties(obj);

            //if (!data.IsArray)
            //{
            //    Debug.LogError("json is not array " + obj.GetType().ToString());
            //    return;
            //}

            //if (!ZPropertyMesh.IsPropertable(typeof(object))){
            //    ConvertCommonType()
            //    return;
            //}


            foreach (var p in props) {
                try
                {
                    if (!((IDictionary)data).Contains(p.PropertyID))
                    {
                        //Debug.LogWarning("json is not have property " + p.PropertyID);
                        continue;
                    }

                }
                catch
                {
                    Debug.LogError("json is not IDictionary " + p.PropertyID);
                    continue;
                }

                var data1 = data[p.PropertyID];

                if (data1 == null)
                    continue;

                ConvertProperty(p, data1);
            }

            ZPropertyMesh.InvokeLoadMethod(obj);

        }

        void ConvertProperty(IZProperty p, JsonData data1)
        {


            if (ZPropertyMesh.IsPropertyList(p))
            {
               // var data1 = data[p.PropertyID];

                ConvertArrayToObjects(p as IZPropertyList, data1);
                return;
            }

            if (ZPropertyMesh.IsPropertyRefList(p))
            {
                //var data1 = data[p.PropertyID];

                ConvertToRefList(p as IZPropertyRefList, data1);
                return;
            }

            if (ZPropertyMesh.IsRankable(p))
            {
                //var data1 = data[p.PropertyID];

                ConvertArrayToRank(p as IRankable, data1);
                return;
            }

            if (ZPropertyMesh.IsEnum(p))
            {

                ConvertEnum(p, data1);
                return;
            }

            if (ZPropertyMesh.IsMultiEnum(p))
            {
                ConvertMultiEnum(p, data1);
                return;
            }

            if (ZPropertyMesh.IsRef(p))
            {
                //var data1 = data[p.PropertyID];
                ConvertRef(p as IRefable, data1);
                return;
            }

            if (ZPropertyMesh.IsNullValue(p))
            {
                return;
            }

            if (ZPropertyMesh.IsUnitValue(p))
            {
                p.Value = Unit.Default;
                return;
            }

            if (IsRawDataPref(p))
            {
                //var data1 = data[p.PropertyID];
                ConvertRawData(p, data1);
                return;
            }

            //              if (p.Value == null)    //interface but not to set;
            //                  continue;
            if (ZPropertyMesh.IsDefineInterface(p) && p.Value == null)
            {
                if (((JsonData)data1).Count <= 0)
                    return;

                var typeStr = (string)((JsonData)data1)["Interface.Type"];
                var interfaceType = Type.GetType(typeStr);

                //try
                {
                    if (interfaceType == null)
                    {
                        var interfaceAssemblyPath = (string)((JsonData)data1)["Interface.AssemblyPath"];

                        Assembly asmb = AssemblyTools.LoadFrom(interfaceAssemblyPath);
                        interfaceType = asmb?.GetType(typeStr);

                    }


                    if (interfaceType == null)
                    {
                        Assembly asmb = AssemblyTools.LoadFrom("Assembly-CSharp.dll");
                        interfaceType = asmb?.GetType(typeStr);
                        //return;
                    }
                }
                // catch (Exception e)
                // {
                //     Debug.LogWarning("ZPropertyJsonLitPrefs Load Dll Error " + e.ToString());
                // }


                var obj = ZPropertyMesh.CreateObject(interfaceType);

                ConvertToObject(obj, (JsonData)data1);
                p.Value = obj;
                //(((JsonData)data1 as IDictionary)?.Keys as IEnumerable).ToList()[0]
                // ZPropertyMesh.GetPropertyType();

                return;
            }


            if (p.Value != null
                && (ZPropertyMesh.IsPropertable(p.GetDefineType())
                    || ZPropertyMesh.IsPropertable(p.Value.GetType()))
            )
            {
                //var data1 = (JsonData)(data[p.PropertyID]);
                var sub = (p).Value;
                ConvertToObject(sub, (JsonData)data1);
            }
            else
            {
                //ret [p.PropertyID] = p.Value;

                //var data1 = data[p.PropertyID];

                if (IsVector2(p))
                {
                    ConvertVector2(p, data1);
                    return;
                }

                if (IsVector3(p))
                {
                    ConvertVector3(p, data1);
                    return;
                }

                if (IsRotation(p))
                {
                    ConvertRotation(p, data1);
                    return;
                }

                if (data1.IsDouble)
                {
                    float a = (float)(double)data1;
                    p.Value = a;
                }
                else if (IsUint(p) && data1.IsInt)
                {
                    p.Value = (uint)(int)data1;
                }

                else if (IsShort(p) && data1.IsInt)
                {
                    p.Value = (short)(int)data1;
                }
                else if (IsLong(p) && data1.IsInt)
                {
                    p.Value = (long)(int)data1;
                }
                else if (data1.IsInt)
                    p.Value = (int)data1;
                else if (data1.IsBoolean)
                    p.Value = (bool)data1;
                else if (data1.IsLong)
                    p.Value = (long)data1;
                else if (data1.IsString)
                    p.Value = (string)data1;
                else if (data1.IsArray)
                    ConvertArrayToObjects(p as IZPropertyList, data1);

                //set runtime properyt's curValue
                if (ZPropertyAttributeTools.IsDefaultToValue(p))
                {
                    (p as IRuntimable).CurValue = p.Value;
                }

            }
        }


        /// <summary>
        /// Converts the array to objects.
        /// </summary>
        private void ConvertArrayToObjects(IZPropertyList obj, JsonData data) {
            if (obj != null) {

                obj.ClearAll();

                Type subType = (obj as IZProperty).GetDefineType();

                bool bClassSub =
                    ZPropertyMesh.IsPropertableLowAPI(subType);

                bool bVector2 = subType == typeof(Vector2);

                foreach (JsonData data1 in data) {

                    if (bVector2) {
                        obj.Add(ConvertVector2(data1));
                        continue;
                    }

                    if (bClassSub) {
                        var sub = ZPropertyMesh.CreateObject(subType);
                        ConvertToObject(sub, (JsonData)data1);
                        obj.Add(sub);
                    }
                    else if (IsUint(obj as IZProperty) && data1.IsInt)
                    {
                        obj.Add((uint)(int)data1);
                    }
                    else if (IsShort(obj as IZProperty) && data1.IsInt)
                    {
                        obj.Add((short)(int)data1);
                    }
                    else if (IsLong(obj as IZProperty) && data1.IsInt)
                    {
                        obj.Add((long)(int)data1);
                    }
                    else if (data1.IsDouble)
                        obj.Add((float)(double)data1);
                    else if (data1.IsInt)
                        obj.Add((int)data1);
                    else if (data1.IsBoolean)
                        obj.Add((bool)data1);
                    else if (data1.IsLong)
                        obj.Add((long)data1);
                    else if (data1.IsString)
                        obj.Add((string)data1);
                }
            }
        }

        /// <summary>
        /// Converts to reference list.
        /// </summary>
        private void ConvertToRefList(IZPropertyRefList refsObj, JsonData data)
        {
            if (refsObj != null)
            {
                refsObj.ClearAll();

                foreach (JsonData data1 in data)
                {
                    int id = (int)(data1["refid"]);
                    //refObj.RefID = id;
                    refsObj.Add(id);
                }

                //to bind the ref when load json file
                refsObj.BindRefs();
            }
        } 

        /// <summary>
        /// Converts the reference.
        /// </summary>
        private void ConvertRef(IRefable refObj, JsonData data)
        {
            //if (!data.IsArray)
                //Debug.LogError("ConvertRef error it is not a ref [PropID]:" + (refObj as IZProperty).PropertyID);


            if (refObj != null)
            {
                int id = (int)(data["refid"]);
                refObj.RefID = id;

                //refObj.BindRef();
            }
        }

        /// <summary>
        /// Converts the array to rank.
        /// </summary>
        private void ConvertArrayToRank(IRankable obj, JsonData data){
            if (!data.IsArray)
            {
                Debug.LogWarning("ConvertArrayToRank error it is not a rank [PropID]:" + (obj as IZProperty).PropertyID);
                var p = (obj as IZProperty);
                //only to read the current value
                //ConvertProperty((obj as IZProperty), data);
                if (p.GetDefineType()  == typeof(Vector2) )
                {
                    p.Value = (ConvertVector2(data));
                }

                if (ZPropertyMesh.IsPropertableLowAPI(p.GetDefineType()))
                {
                    var sub = ZPropertyMesh.CreateObject(p.GetDefineType());
                    ConvertToObject(sub, (JsonData)data);
                    p.Value  = sub;
                }
                else if (IsUint(obj as IZProperty) && data.IsInt)
                {
                    p.Value = (uint)(int)data;
                }
                else if (IsShort(obj as IZProperty) && data.IsInt)
                {
                    p.Value = (short)(int)data;
                }
                else if (IsLong(obj as IZProperty) && data.IsInt)
                {
                    p.Value = (long)(int)data;
                }
                else if (data.IsDouble)
                    p.Value = (float)(double)data;
                else if (data.IsInt)
                    p.Value = (int)data;
                else if (data.IsBoolean)
                    p.Value = (bool)data;
                else if (data.IsLong)
                    p.Value = (long)data;
                else if (data.IsString)
                    p.Value = (string)data;

                return;
            }

			if (obj != null) {

				Type subType = (obj as IZProperty).GetDefineType ();

				bool bClassSub = 
					ZPropertyMesh.IsPropertableLowAPI (subType);

				bool bVector2 = subType == typeof(Vector2);

				foreach (JsonData data1 in data) {	

					if (bVector2) {
						obj.AddRank( ConvertVector2 (data1));
						continue;
					}

					if (bClassSub) {
						var sub = ZPropertyMesh.CreateObject (subType);
						ConvertToObject(sub, (JsonData)data1);
						obj.AddRank (sub);
					}
                    else if (IsUint(obj as IZProperty) && data1.IsInt)
                    {
                        obj.AddRank((uint)(int)data1);
                    }
                    else if (IsShort(obj as IZProperty) && data1.IsInt)
                    {
                        obj.AddRank((short)(int)data1);
                    }
                    else if (IsLong(obj as IZProperty) && data1.IsInt)
                    {
                        obj.AddRank((long)(int)data1);
                    }
                    else if (data1.IsDouble)
						obj.AddRank((float)(double)data1);
					else if (data1.IsInt)
						obj.AddRank((int)data1);
					else if (data1.IsBoolean)
						obj.AddRank((bool)data1);
					else if (data1.IsLong)
						obj.AddRank((long)data1);
					else if (data1.IsString)
						obj.AddRank((string)data1);
				}
			}
		}



		/// <summary>
		/// Determines if is vector2 the specified prop.
		/// </summary>
		static public bool IsVector2(IZProperty prop){
			return prop.GetDefineType () == typeof(Vector2);
		}

		/// <summary>
		/// Converts the vector2.
		/// </summary>
		static public void ConvertVector2(IZProperty prop, JsonData data){
            //if (!data.IsArray)
                //Debug.LogError("ConvertVector2 error it is not a vector [PropID]:" + (prop).PropertyID);


            float xData = (float)(double)(data["x"]);
			float yData = (float)(double)(data["y"]);

			Vector2 v2 = new Vector2((float)xData, (float)yData);

			prop.Value = v2;
		}
	

		static public Vector2 ConvertVector2(JsonData data){

            if (data.Count < 2)
                Debug.LogError("ConvertVector2 error it is not a vector");


            float xData = (float)(double)(data["x"]);
			float yData = (float)(double)(data["y"]);

			Vector2 v2 = new Vector2((float)xData, (float)yData);

			return v2;
		}


        static public bool IsUint(IZProperty prop)
        {
            return prop.GetDefineType() == typeof(uint);
        }

        static public bool IsLong(IZProperty prop)
        {
            return prop.GetDefineType() == typeof(long);
        }

        static public bool IsShort(IZProperty prop)
        {
            return prop.GetDefineType() == typeof(short);
        }



        /// <summary>
        /// Determines if is vector3 the specified prop.
        /// </summary>
        static public bool IsVector3(IZProperty prop){
			return prop.GetDefineType () == typeof(Vector3);
		}

		/// <summary>
		/// Converts the vector3.
		/// </summary>
		static public void ConvertVector3(IZProperty prop, JsonData data){
            if (data.Count<3)
                Debug.LogError("ConvertVector3 error it is not a vector [PropID]:" + (prop).PropertyID);


            float xData = (float)(double)(data["x"]);
			float yData = (float)(double)(data["y"]);
			float zData = (float)(double)(data["z"]);

			Vector3 v3 = new Vector3((float)xData, (float)yData, zData);

			prop.Value = v3;
		}

        static public Vector3 ConvertVector3(JsonData data)
        {
            //if (data.Count < 3)
                //Debug.LogError("ConvertVector3 error it is not a vector [PropID]:" + (prop).PropertyID);


            float xData = (float)(double)(data["x"]);
            float yData = (float)(double)(data["y"]);
            float zData = (float)(double)(data["z"]);

            Vector3 v3 = new Vector3((float)xData, (float)yData, zData);

            return v3;
        }

        static public bool IsRotation(IZProperty prop)
        {
            return prop.GetDefineType() == typeof(Quaternion);
        }

        static public void ConvertRotation(IZProperty prop, JsonData data)
        {
            if (data.Count < 4)
                Debug.LogError("ConvertRotation error it is not a Quaternion [PropID]:" + (prop).PropertyID);


            float xData = (float)(double)(data["x"]);
            float yData = (float)(double)(data["y"]);
            float zData = (float)(double)(data["z"]);
            float wData = (float)(double)(data["w"]);

            Quaternion q = new Quaternion((float)xData, (float)yData, zData, wData);

            prop.Value = q;
        }


        static public bool IsRawDataPref(IZProperty prop)
        {
            return typeof(IRawDataPref).IsAssignableFrom(prop.GetDefineType());
        }


        static public void ConvertRawData(IZProperty prop, JsonData data)
        {
            var raw = new JsonLitRawData();
            raw.RawData = data;
            prop.Value = raw;
        }


        static public void ConvertEnum(IZProperty prop, JsonData data)
        {
            if (!data.IsString)
                Debug.LogError("ConvertEnum error it is not a enum string [PropID]:" + (prop).PropertyID);

            try
            {

                prop.Value = Enum.Parse(prop.GetDefineType(), (string)data, true);
            }
            catch 
            {
                Debug.LogError("Enum.Parse error " + prop.PropertyID + " data= " + data );
            }

        }

        static public void ConvertMultiEnum(IZProperty prop, JsonData data)
        {
            if (!data.IsString)
                Debug.LogError("ConvertEnum error it is not a enum string [PropID]:" + (prop).PropertyID);

            try
            {

                //prop.Value = Enum.Parse(prop.GetDefineType(), (string)data, true);

                prop.Value = (prop.Value as IMultiEnumerable).Parse((string)data);
            }
            catch
            {
                Debug.LogError("MultiEnum.Parse error " + prop.PropertyID);
            }

        }

        public bool UpdateList<T>(ZPropertyRefList<T> list, string updateJsonStr)
        {
            return ZPropertyJsonLitUpdater.UpdateList<T>(list, updateJsonStr);
        }

        public void LoadFromRawData(object obj, IRawDataPref raw)
        {

            var json = raw?.RawData as JsonData;

            if (json != null)
            {
                ConvertToObject(obj, json);
            }
        }

        public object LoadValueFromRawData(Type type, IRawDataPref raw)
        {
            var json = raw?.RawData as JsonData;

            if (type == typeof(Vector2))
            {
                return ConvertVector2(json);
            }
            else if (type == typeof(Vector3))
            {
                return ConvertVector3(json);
            }
            else if (type == typeof(uint) && json.IsInt)
            {
                return (uint)(int)json;
            }
            else if (type == typeof(long) && json.IsInt)
            {
                return (long)(int)json;
            }
            else if (type == typeof(short) && json.IsInt)
            {
                return (short)(int)json;
            }
            else if (json.IsBoolean)
                return (bool)json;
            else if (json.IsDouble)
                return (double)json;
            else if (json.IsInt)
                return (int)json;
            else if (json.IsLong)
                return (long)json;

            return null;
        }

        public IRawDataPref ConvertToRawData(object obj)
        {
            JsonLitRawData ret = null;
            if ( ZPropertyMesh.IsValueType(obj.GetType())){
                ret = new JsonLitRawData(obj);

                return ret;
            }

            var map = ZPropertyMapper.ConvertToMap(obj);
            if (map == null)
                return null;

            ret = new JsonLitRawData(map);

            return ret;
        }

        public string ConvertToStr(object obj)
        {
            if (obj.GetType() == typeof(string))
            {
                strTestData = (string)obj;
            }
           else if (typeof(ICollection).IsAssignableFrom(obj.GetType()))
            {
                strTestData = JsonMapper.ToJson(obj);
            }
            else if (ZPropertyMesh.IsPropertyList(obj as IZProperty))
            {
                var map = ZPropertyMapper.ConvertToList(obj);
                strTestData = JsonMapper.ToJson(map);
            }
            else
            {
                var map = ZPropertyMapper.ConvertToMap(obj);
                strTestData = map != null ? JsonMapper.ToJson(map) : "";
            }


            return strTestData;
        }
    }
}

