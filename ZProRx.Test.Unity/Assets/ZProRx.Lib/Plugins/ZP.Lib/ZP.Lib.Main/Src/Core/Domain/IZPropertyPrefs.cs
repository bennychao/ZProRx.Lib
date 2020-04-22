using System;
using System.Collections.Generic;

namespace ZP.Lib
{
    public interface IZPropertyPrefs
	{
		void Save(object obj, string path);
			
		void Load(object obj, string path);

		void LoadFromStr(object obj, string strData);

        void LoadValueFromStr(IZProperty property, string strData);

        string ConvertToStr(object obj);

        IRawDataPref ConvertToRawData(object obj);

        bool UpdateList<T>(ZPropertyRefList<T> list, string updateJsonStr);

        void LoadFromRawData(object obj, IRawDataPref raw);

        object LoadValueFromRawData(Type type, IRawDataPref raw);

    }
}

