using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.Core.Domain;

namespace ZP.Lib.Core.Main
{
    public class PropBehaviourSingletonWithRoom<T> : MonoBehaviour where T : PropBehaviourSingletonWithRoom<T>
    {
        protected static Dictionary<int, T> _InstanceMap = new Dictionary<int, T>();

        public static T Instance
        {
            get
            {
                var id = (TaskScheduler.Current as IZMatrixRuntime)?.RoomId ?? -1;

                if (id < 0)
                {
                    throw new Exception("Call Not In MatrixRuntime");
                }
                T tempInstance = null;
                if (!_InstanceMap.TryGetValue(id, out tempInstance))
                {
                    tempInstance = _InstanceMap[id] = FindObjectOfType<T>();
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
            if (!_InstanceMap.TryGetValue(id, out tempInstance))
            {
                ZPropertyMesh.ReleaseObject(tempInstance);
                _InstanceMap[id] = null;

                tempInstance.OnRelease();
            }
        }
    }
}
