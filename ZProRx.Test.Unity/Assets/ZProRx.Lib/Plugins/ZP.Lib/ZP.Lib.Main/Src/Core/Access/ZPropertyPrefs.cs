using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZP.Lib
{
	
	public static class ZPropertyPrefs  {
		static IZPropertyPrefs prefs;

		static ZPropertyPrefs(){
			prefs = new ZPropertyJsonLitPrefs ();
		}

		static public void Save(object obj, string path)
		{
			prefs.Save (obj,path);
		}

		static public void Load(object obj, string path)
		{
			prefs.Load(obj, path);
		}

		static public void LoadFromStr(object obj, string strData)
		{
            if (!string.IsNullOrEmpty(strData))
            {
                prefs.LoadFromStr(obj, strData);

                ZPropertyMesh.InvokeLoadMethod(obj);
            }

		}

        static public void LoadValueFromStr(IZProperty p, string path)
        {
            prefs.LoadValueFromStr(p, path);

            ZPropertyMesh.InvokeLoadMethod(p.Value);
        }

        static public void LoadFromRes(object obj, string path)
        {
#if !ZP_SERVER
            var resPath = path.Replace(".json", "");
#else
            var resPath = path;
#endif
            var res = Resources.Load<TextAsset>(resPath);
            if (res != null)
            {
                prefs.LoadFromStr(obj, res.text);

                ZPropertyMesh.InvokeLoadMethod(obj);
            }

        }

        static public string ConvertToStr(object obj)
        {
            if (obj == null)
                return "";

            return prefs.ConvertToStr(obj);
        }

        static public IRawDataPref ConvertToRawData(object obj)
        {
            return prefs.ConvertToRawData(obj);
        }
        static public bool UpdateList<T>(ZPropertyRefList<T> list, string updateJsonStr)
        {
            return prefs.UpdateList<T>(list, updateJsonStr);
        }

        static public IZPropertyPrefs GetRrefs()
        {
            return prefs;
        }

        static public void LoadFromRawData(object obj, IRawDataPref raw)
        {
            prefs.LoadFromRawData(obj, raw);
        }

        static public object LoadValueFromRawData(Type type, IRawDataPref raw)
        {
            return prefs.LoadValueFromRawData(type, raw);
        }

    }
}

