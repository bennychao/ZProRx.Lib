using LitJson;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;
using ZP.Lib;

namespace ZP.Lib.Core
{
    public class JsonLitRawData : IRawDataPref
    {
        JsonData data;
        public object RawData
        {
            get
            {
                return data;
            }

            set
            {
                data = (JsonData)value; //new JsonData(value);
            }
        }

        public JsonLitRawData()
        {

        }

        public JsonLitRawData(object obj)
        {
            //data = new JsonData();

            if (obj is int)
                data = new JsonData((int)obj);
            else if (obj is string)
                data = new JsonData((string)obj);

            else if (obj is long)
                data = new JsonData((long)obj);

            else if (obj is bool)
                data = new JsonData((bool)obj);

            else if (obj is double)
                data = new JsonData((double)obj);

            else if (obj is float)
                data = new JsonData((double)(float)obj);
            else
                data = new JsonData(obj);

        }



        public JsonLitRawData(Dictionary<string, object> map)
        {
            data = ConvertToJsonData(map);
        }

        public T QueryProperty<T>(string propertyId)
        {
            var dic = data as IDictionary;
            if (dic == null)
                return default(T);

            foreach (var key in dic.Keys)
            {
                if (((string)key).Contains(propertyId))
                {
                    return (T)(object)data[propertyId];
                }
            }


            return default(T);
        }
        
        public object ToValue()
        {
            if (data.IsObject)
            {
                Dictionary<string, object> rets = new Dictionary<string, object>();

                rets = ToMapSub(data);

                return rets;
            }
            else if (data.IsArray)
            {
                return ToListSub(data);
            }
            else if (data.IsBoolean)
                return (bool)data;
            else if (data.IsDouble)
                return (double)data;
            else if (data.IsString)
                return (string)data;
            else if (data.IsInt)
                return (int)data;
            else if (data.IsLong)
                return (long)data;
            else
                return (object)data;
        }

        private static Dictionary<string, object> ToMapSub(JsonData jdata)
        {
            Dictionary<string, object> rets = new Dictionary<string, object>();

            var dic = jdata as IDictionary;
            Assert.IsNotNull(dic, "Map Error is Not a KeyData");

            foreach (var key in dic.Keys)
            {
                var json = jdata[key.ToString()];
                if (json.IsBoolean)
                    rets[key.ToString()] = (bool)json;
                else if (json.IsDouble)
                    rets[key.ToString()] = (double)json;
                else if (json.IsString)
                    rets[key.ToString()] = (string)json;
                else if (json.IsInt)
                    rets[key.ToString()] = (int)json;
                else if (json.IsLong)
                    rets[key.ToString()] = (long)json;
                else if (json.IsObject)
                    rets[key.ToString()] = ToMapSub(json);
                else if (json.IsArray)
                    rets[key.ToString()] = ToListSub(json);
                else
                    rets[key.ToString()] = ToMapSub(json);
            }

            return rets;

        }

        private static List<object> ToListSub(JsonData jdata)
        {
            List<object> rets = new List<object>();

            var list = jdata as IList;

            for( int i = 0; i < list.Count; i++)
            {
                var json = jdata[i];
                if (json.IsBoolean)
                    rets.Add((bool)json);
                else if (json.IsDouble)
                    rets.Add((double)json);
                else if (json.IsString)
                    rets.Add((string)json);
                else if (json.IsInt)
                    rets.Add((int)json);
                else if (json.IsLong)
                    rets.Add((long)json);
                else if (json.IsObject)
                    rets.Add(ToMapSub(json));
                else if (json.IsArray)
                    rets.Add(ToListSub(json));
                else
                    rets.Add(ToMapSub(json));
            }

            return rets;
        }

        private JsonData ConvertToJsonData(Dictionary<string, object> map)
        {
            var dataRet = new JsonData();
            //data.SetJsonType(JsonType.)
            foreach (var k in map)
            {
                if (k.Value is int)
                    dataRet[k.Key] = new JsonData((int)k.Value);
                else if (k.Value is string)
                    dataRet[k.Key] = new JsonData((string)k.Value);

                else if (k.Value is long)
                    dataRet[k.Key] = new JsonData((long)k.Value);

                else if (k.Value is bool)
                    dataRet[k.Key] = new JsonData((bool)k.Value);

                else if (k.Value is double)
                    dataRet[k.Key] = new JsonData((double)k.Value);

                else if (k.Value is float)
                    dataRet[k.Key] = new JsonData((double)(float)k.Value);

                else if (k.Value is List<object>)
                    dataRet[k.Key] = ConvertListToJsonData(k.Value as List<object>);

                else if (k.Value is Dictionary<string, object>)
                    dataRet[k.Key] = ConvertToJsonData(k.Value as Dictionary<string, object>);
                else
                    dataRet[k.Key] = new JsonData(k.Value);
            }

            return dataRet;
        }

        private JsonData ConvertListToJsonData(List<object> list)
        {
            var dataRet = new JsonData();

            foreach (var obj in list)
            {
                if (obj is int)
                    dataRet.Add((int)obj);
                else if (obj is string)
                    dataRet.Add((string)obj);

                else if (obj is long)
                    dataRet.Add((long)obj);

                else if (obj is bool)
                    dataRet.Add((bool)obj);

                else if (obj is double)
                    dataRet.Add((double)obj);

                else if (obj is float)
                    dataRet.Add((double)(float)obj);

                else if (obj is List<object>)
                    dataRet.Add( ConvertListToJsonData(obj as List<object>));

                else if (obj is Dictionary<string, object>)
                    dataRet.Add(ConvertToJsonData(obj as Dictionary<string, object>));
                else
                    dataRet.Add(obj);
            }

            return dataRet;
        }

        private JsonData ConvertObjectToJsonData(object obj)
        {
            var dataRet = new JsonData();
            if (obj is int)
                dataRet = new JsonData((int)obj);
            else if (obj is string)
                dataRet = new JsonData((string)obj);

            else if (obj is long)
                dataRet = new JsonData((long)obj);

            else if (obj is bool)
                dataRet = new JsonData((bool)obj);

            else if (obj is double)
                dataRet = new JsonData((double)obj);

            else if (obj is float)
                dataRet = new JsonData((double)(float)obj);
            else
                dataRet = new JsonData(obj);

            return dataRet;
        }

        public T GetData<T>()
        {
            var ret = ZPropertyMesh.CreateObject<T>();

            ZPropertyPrefs.LoadFromRawData(ret, this);

            return ret;
        }
    }
}
