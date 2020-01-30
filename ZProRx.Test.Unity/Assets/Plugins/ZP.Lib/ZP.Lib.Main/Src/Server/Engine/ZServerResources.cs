using System;
#if ZP_SERVER
using ZP.Lib;

namespace UnityEngine
{
    internal sealed class ZServerResources
    {
        public ZServerResources()
        {
        }

        //support the Prefabs GameObject
        static public GameObject Load(string path)
        {
            return ZPrefabsMgr.Instance.FindPrefabByPath(path);
            //return null;
        }

        static public ResourceRequest LoadAsync(string path)
        {
            return null;
        }
    }
}

#endif
