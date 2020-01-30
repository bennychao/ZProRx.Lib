using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib;
using ZP.Lib.CoreEx.Reactive;
using ZP.Lib.Core.Domain;

namespace ZP.Lib.Core.Main
{
    public class PropObjectSingletonWithRoom<T> where T : PropObjectSingletonWithRoom<T>
    {
        protected volatile static ConcurrentDictionary<int, T> _InstanceMap = new ConcurrentDictionary<int, T>();

        protected TaskScheduler curScheduler = null;

        protected IScheduler rxScheduler = null;

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
                    lock(typeof(PropObjectSingletonWithRoom<T>))
                    {
                        if (!_InstanceMap.TryGetValue(id, out tempInstance))
                        {
                            tempInstance = _InstanceMap[id] = ZPropertyMesh.CreateObject<T>();

                            tempInstance.curScheduler = TaskScheduler.Current;

                            tempInstance.rxScheduler = new ZRuntimeRxScheduler(TaskScheduler.Current);
                        }
                    }
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

        public (IDisposable cancellationDisposable, Task task) RunInCurrentRoom(Action action)
        {
            var d = new BooleanDisposable();

            var t = new Task(() =>
            {
                if (!d.IsDisposed)
                {
                    action();
                }

            });
            
            t?.Start(curScheduler);

            return (d, t) ;
        }
    }
}
