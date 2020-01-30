using System;
using UnityEngine;

namespace ZP.Lib
{
    internal class ZPrefabsMgr : PropObjectSingleton<ZPrefabsMgr>
    {
        public ZPropertyList<ZPrefab> Prefabs = new ZPropertyList<ZPrefab>();

        public ZPrefabsMgr()
        {
        }

        public GameObject FindPrefabByName(string name)
        {
            return Prefabs.FindValue((arg) => 
            string.Compare(arg.gameObject.Name, name, StringComparison.Ordinal) == 0)?.gameObject.Value;
        }


        public GameObject FindPrefabByPath(string path)
        {
            return Prefabs.FindValue((arg) =>
            string.Compare(arg.path, path, StringComparison.Ordinal) == 0)?.gameObject.Value;
        }

        public void Clear()
        {
            Prefabs.ClearAll();
        }
    }
}
