using System;
using System.IO;
#if ZP_SERVER

namespace UnityEngine
{
    public sealed class Resources
    {
        static public T Load<T>(string path)
        {
            if (typeof(T) == typeof(TextAsset)) {
                TextAsset text = new TextAsset(ServerPath.Instance.RESOURCES + path, true);

                return (T)(object)text;
            }
            return default(T);
        }

        static public T[] LoadAll<T>(string path)
        {
            //string 

            var fs = Directory.GetFiles(ServerPath.Instance.RESOURCES + path, "*.json");
            //var files = Directory.GetFiles("C:\\path", "*.*", SearchOption.AllDirectories)
            //.Where(s => s.EndsWith(".bmp") || s.EndsWith(".jpg"));

            T[] ts = new T[fs.Length];

            int index = 0;
            foreach (var f in fs) {
                TextAsset text = new TextAsset(f, true);
                ts[index++] = (T)(object)text;
            }


            return ts;
        }
    }
}
#endif