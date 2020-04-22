using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib;
using ZP.Lib.Core.Pool;
using ZP.Lib.CoreEx;

namespace ZP.Lib.Core.Main
{

    public class PropObjectSinglenodeWithTaskScheduler<T> where T : class //PropObjectSingletonWithTaskScheduler<T>
    {
        protected volatile static Dictionary<int, SinglenodeItem<T> > _InstanceMap = new Dictionary<int, SinglenodeItem<T> >();

        protected TaskScheduler curScheduler = null;

        protected IScheduler rxScheduler = null;

        public IScheduler RxScheduler => rxScheduler;

        public static T Instance
        {
            get
            {
                var id = TaskScheduler.Current.Id;
                SinglenodeItem<T> tempInstance = null;
                if (!_InstanceMap.TryGetValue(id, out tempInstance))
                {
                    lock (typeof(PropObjectSinglenodeWithTaskScheduler<T>))
                    {
                        if (!_InstanceMap.TryGetValue(id, out tempInstance))
                        {
                            _InstanceMap[id] = tempInstance = new SinglenodeItem<T>();

                            tempInstance.Instance = ZPropertyMesh.CreateObject<T>();

                            tempInstance.curScheduler = TaskScheduler.Current;

                            tempInstance.rxScheduler = new ZRuntimeRxScheduler(TaskScheduler.Current);
                        }
                    }
                }

                return tempInstance.Instance;
            }
        }

        //public virtual void OnRelease()
        //{

        //}

        public static void Release()
        {
            var id = TaskScheduler.Current.Id;
            Release(id);
        }

        public static void Release(int id)
        {
            SinglenodeItem<T> tempInstance = null;
            if (_InstanceMap.TryGetValue(id, out tempInstance))
            {
                ZPropertyMesh.ReleaseObject(tempInstance.Instance);
                _InstanceMap[id] = null;

                //tempInstance.OnRelease();
            }
        }

        public IDisposable RunInCurrentTaskScheduler(Action action)
        {
            var d = new BooleanDisposable();

            new Task(() =>
            {
                if (!d.IsDisposed)
                {
                    action();
                }

            }).Start(curScheduler);

            return d;
        }
    }


    public class PropObjectSingletonWithTaskScheduler<T> where T : PropObjectSingletonWithTaskScheduler<T>
    {
        protected volatile static Dictionary<int, T> _InstanceMap = new Dictionary<int, T>();

        protected TaskScheduler curScheduler = null;

        protected IScheduler rxScheduler = null;

        public IScheduler RxScheduler => rxScheduler;

        public static T Instance
        {
            get
            {
                var id =TaskScheduler.Current.Id;
                T tempInstance = null;
                if (!_InstanceMap.TryGetValue(id, out tempInstance))
                {
                    lock(typeof(PropObjectSingletonWithTaskScheduler<T>))
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

        public static void ReleaseAll()
        {
            foreach (var item in _InstanceMap.Values)
            {
                item.OnRelease();

            }

            _InstanceMap.Clear();
        }

        public static void Release(int id)
        {
            T tempInstance = null;
            if (_InstanceMap.TryGetValue(id, out tempInstance))
            {   
                ZPropertyMesh.ReleaseObject(tempInstance);
                _InstanceMap[id] = null;
                _InstanceMap.Remove(id);

                tempInstance.OnRelease();
            }
        }

        public IDisposable RunInCurrentTaskScheduler(Action action)
        {
            var d = new BooleanDisposable();

            new Task(() =>
            {
                if (!d.IsDisposed)
                {
                    action();
                }

            }).Start(curScheduler);

            return d;
        }

        [Conditional("DEBUG")]
        static public void CheckMapCount(int count)
        {
            //return _InstanceMap.Count;
            if (_InstanceMap.Count != count)
                throw new Exception("Check Map Count Error");
        }
    }
}
