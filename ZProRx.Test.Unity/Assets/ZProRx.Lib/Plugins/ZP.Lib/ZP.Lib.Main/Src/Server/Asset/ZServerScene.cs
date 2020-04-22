using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UniRx;
using System.Linq;
using ZP.Lib.Core.Main;
using System.Threading.Tasks;
using ZP.Lib.CoreEx;

#if ZP_SERVER

namespace ZP.Lib
{
    public sealed class ZServerScene  : PropObjectSingletonWithRoom<ZServerScene>
    {

        public ZProperty<string> Name = new ZProperty<string>();

        internal ZPropertyList<GameObject> Objects = new ZPropertyList<GameObject>();

        public int Frame => frame;     

        private int frame = 0;

        public void Load(string name)
        {
            lock(this)
            {
                if (string.Compare(name, Name.Value) == 0)
                    return;

                Close();

                //var s = ZServerScene.Instance;
                var p = ZPrefabsMgr.Instance;

                ZPropertyPrefs.Load(p, ServerPath.Instance.PREFABS);

                ZPropertyPrefs.Load(this, ServerPath.Instance.SCENE_ROOT + name + ".json");

                //Destroy current Scene
            }
        }

        public void Close()
        {
            var allObjs = GetAllComponents();

            foreach (var o in allObjs)
            {
                InvokeOnDestroyMethod(o);
            }
            Objects.ClearAll();
        }


        public Task LoadAsync(string name)
        {
            lock(this)
            {
                if (string.Compare(name, Name.Value) == 0)
                    return Task.CompletedTask;
            }

            //current thread
            //Task.Run(() => Load(name));

            //Close();

           return  RunInCurrentRoom(() => Load(name)).task;
        }

        //auto call after load json
        void OnLoad()
        {
            Debug.Log($"Scene {Name} OnLoad ");
            foreach (var o in Objects)
            {
                o.Load();
            }

            var allObjs = GetAllComponents();

            //awake the scene
            foreach (var o in allObjs)
            {
                InvokeAwakeMethod(o);
            }


            foreach (var o in allObjs)
            {
                InvokeStartMethod(o);
            }


            Observable.Interval(TimeSpan.FromSeconds(0.1f)).ObserveOn(rxScheduler).Subscribe(_ =>
            {
                //Debug.Log("Interval");
                
                foreach (var o in allObjs)
                {
                    InvokeUpdateMethod(o);
                }

                frame++;

                //update object
                allObjs = GetAllComponents();

                //physics update
                Physics.FixedUpdate(this);
            });
        }

        internal List<GameObject> GetAllObjects()
        {
            List<GameObject> ret = new List<GameObject>();
            foreach (var o in Objects)
            {
                ret.AddRange(o.GetAllChildren());
            }

            ret.AddRange((Objects as IZPropertyList<GameObject>).ConvertToArray().ToList());

            return ret;
        }


        internal List<T> GetAllComponents<T>() where T : ZServerComponent
        {
            List<T> coms = new List<T>();
            var objs = GetAllObjects();
            foreach (var o in objs)
            {
                //coms.AddRange(o.GetComponents<T>());

                coms.AddRange(o.GetComponents<T>());
            }
            return coms;
        }

        internal void AddGameObject(GameObject col)
        {
            var parent = col.transform.parent;
            //update the scene's object

            if (parent == null)
            {
                this.Objects.Add(col);
            }

            //Awake and Start it

            InvokeAwakeMethod(col);

            InvokeStartMethod(col);
        }

        private List<ZServerComponent> GetAllComponents()
        {
            List<ZServerComponent> coms = new List<ZServerComponent>();
            var objs = GetAllObjects();
            foreach (var o in objs)
            {
                //coms.AddRange(o.GetComponents<ZServerComponent>());

                coms.AddRange(o.GetComponents<ZServerComponent>()); //InChildren
            }
            return coms;
        }


        static private void InvokeAwakeMethod(object obj)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
            object[] parameters = null;

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static private void InvokeStartMethod(object obj)
        {
            Type type = obj.GetType();
            // BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
            MethodInfo method = type.GetMethod("Start", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
            object[] parameters = null;

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static private void InvokeUpdateMethod(object obj)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
            object[] parameters = null;

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static private void InvokeOnDestroyMethod(object obj)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnDestroy", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
            object[] parameters = null;

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }
    }
}
#endif
