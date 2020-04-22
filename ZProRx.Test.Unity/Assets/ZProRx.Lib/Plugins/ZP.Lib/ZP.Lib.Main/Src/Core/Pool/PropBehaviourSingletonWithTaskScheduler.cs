using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Core.Main
{
    public class PropBehaviourSingletonWithTaskScheduler<T> : MonoBehaviour  where T : PropBehaviourSingletonWithTaskScheduler<T>
    {
        protected static Dictionary<int, T> _InstanceMap = new Dictionary<int, T>();

        protected TaskScheduler curScheduler = null;

        protected IScheduler rxScheduler = null;

        public IScheduler RxScheduler => rxScheduler;

        public static T Instance
        {
            get
            {
                var id = TaskScheduler.Current.Id;
                T tempInstance = null;
                if (!_InstanceMap.TryGetValue(id, out tempInstance))
                {
                    tempInstance = _InstanceMap[id] = FindObjectOfType<T>();

                    tempInstance.curScheduler = TaskScheduler.Current;

                    tempInstance.rxScheduler = new ZRuntimeRxScheduler(TaskScheduler.Current);
                }

                return tempInstance;
            }
        }

        public virtual void OnRelease()
        {

        }

        public static void Release()
        {
            var id = TaskScheduler.Current.Id;
            Release(id);
        }

        public static void Release(int id)
        {
            T tempInstance = null;
            if (_InstanceMap.TryGetValue(id, out tempInstance))
            {
                ZPropertyMesh.ReleaseObject(tempInstance);
                _InstanceMap[id] = null;

                tempInstance.OnRelease();
            }
        }
    }
}
