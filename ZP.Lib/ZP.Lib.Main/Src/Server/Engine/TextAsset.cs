using System;
using System.IO;
#if ZP_SERVER
using ZP.Lib;

namespace UnityEngine
{
    public class TextAsset : IBindable
    {
        static char[] PATH_SEPARATOR = { '/', '\\' };
        string strText = "";

        string fileName = "";
        public TextAsset()
        {
            //load
        }

        public TextAsset(string path, bool bFull = false)
        {
            var index = path.LastIndexOfAny(PATH_SEPARATOR);
           
            fileName = path.Substring(index > 0 ? index + 1 : 0);
            var lastIndex = fileName.IndexOf('.');
            fileName = fileName.Remove(lastIndex);

            string fullPath = bFull ? path  : ServerPath.Instance.ASSETS + path;
            //load
            strText = File.ReadAllText(fullPath);
        }

        public string text{
            get => strText;
        }

        public string name
        {
            get => fileName;
        }

        //internal void BindData(BindComponentParam param)
        //{

        //}

        void IBindable.BindData(BindComponentParam param)
        {
            //load the json
            var path = ((string)param.Data.Value);
            strText = File.ReadAllText(ServerPath.Instance.ASSETS + path);

            var index = path.LastIndexOf('/');
            fileName = path.Substring(index > 0 ? index + 1 : 0);
            var lastIndex = fileName.IndexOf('.');
            fileName = fileName.Remove(lastIndex);
        }
    }
}
#endif
