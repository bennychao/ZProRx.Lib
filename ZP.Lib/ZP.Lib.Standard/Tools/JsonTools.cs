
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZP.Lib.Standard.Tools
{
    static public class JsonTools
    {
        static public void Write(string path, string config)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.WriteRaw(config);
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

       static  public void Write<T>(string path, string section, T t)
        {
            try
            {
                JObject jObj;
                using (StreamReader file = new StreamReader(path))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObj = (JObject)JToken.ReadFrom(reader);
                    var json = JsonConvert.SerializeObject(t);
                    if (string.IsNullOrWhiteSpace(section))
                        jObj = JObject.Parse(json);
                    else
                        jObj[section] = JObject.Parse(json);
                }

                using (var writer = new StreamWriter(path))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jObj.WriteTo(jsonWriter);
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
    }
}