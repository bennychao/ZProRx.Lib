using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.Core.Main
{
    public class PropObjectSingletonWIthId<TID, T> where T : PropObjectSingletonWIthId<TID, T> where TID : IComparable
    {

        protected static volatile ConcurrentDictionary<TID, T> _InstanceMap = new ConcurrentDictionary<TID, T>();

        public static T Instance
        {
            get 
            {
                T tempInstance = default(T);
                if (!_InstanceMap.TryGetValue(default(TID) , out tempInstance) )
                {
                    lock (typeof(PropObjectSingletonWIthId<TID, T>))
                    {
                        if (!_InstanceMap.TryGetValue(default(TID), out tempInstance))
                            _InstanceMap[default(TID)] = ZPropertyMesh.CreateObject<T>();
                    }
                }
                return tempInstance;
            }
        }

        public static T GetInstance(TID Id)
        {
            T tempInstance = null;
            if (!_InstanceMap.TryGetValue(Id, out tempInstance))
            {
                lock (typeof(PropObjectSingletonWIthId<TID, T>))
                {
                    if (!_InstanceMap.TryGetValue(Id, out tempInstance))
                    {
                        tempInstance = _InstanceMap[Id] = ZPropertyMesh.CreateObject<T>();
                    }
                }
            }
            return tempInstance;
        }
    }

}
