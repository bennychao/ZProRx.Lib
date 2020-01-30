using System;
namespace ZP.Lib.Common
{
    static public class PathTools
    {
        static public string UpperPath(string path, int count)
        {
            string ret = path;

            for (int i = 0; i < count; i++)
            {
                var pIndex = ret.LastIndexOfAny(new char[] { '/', '\\'});
                ret = ret.Substring(0, pIndex);
            }
            return ret;

        }
    }
}
