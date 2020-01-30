using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using ZP.Lib;

#if ZP_SERVER

namespace UnityEngine
{
    public sealed class GameObject  : ZServerObject
    {
        //ZP 
        private ZProperty<string> InName = new ZProperty<string>();

        private ZProperty<ZTransform3> InTransform = new ZProperty<ZTransform3>();

        private ZPropertyList<GameObject> Objects = new ZPropertyList<GameObject>();

        private ZPropertyList<BindComponent> InComponents = new ZPropertyList<BindComponent>();

        private ZProperty<short> InLayer = new ZProperty<short>();
        //end ZP

        private List<ZServerComponent> components = new List<ZServerComponent>();

        //sub objects


        protected bool bActive = true;

        public string name
        {
            set
            {
                InName.Value = value;
            }
            get
            {
                return InName.Value;
            }
        }

        public short layer
        {
            get => InLayer.Value;
            set => InLayer.Value = value;
        }


        public GameObject gameObject
        {
            get => this;
        }

        public Transform transform
        {
            get
            {
               // LayerMask
                return GetComponent<Transform>();
            }
            //set;
        }

        public List<GameObject> Subs
        {
            get {
                return (Objects as IZPropertyList<GameObject>).ConvertToArray().ToList();
            }
        }

        public GameObject()
        {
        }

        //call by load scene, for scene's init
        public void Load(Transform parent = null)
        {
            //bind transform
            AddComponent<Transform>();

            InTransform.Value.ApplyToLocalTransform(transform);

            foreach (var c in InComponents)
            {
                // AddComponent(c.)
                c.BindGameObject(transform);
            }

            //load subs
            foreach (var o in Objects)
            {
                o.Load(transform);

                //bind parent
                //o.transform.parent = transform;
            }

            transform.parent = parent;
        }


        public static GameObject Instantiate(GameObject prefab, Transform parent  = null)
        {
            //ZPrefabsMgr.Instance.FindPrefabByName()
            var col = ZPropertyMesh.CloneObject(prefab) as GameObject;

            if (col != null)
            {
                col?.Load(parent);

                if (parent != null)
                {
                    parent.gameObject.Objects.Add(col);
                }

                ZServerScene.Instance.AddGameObject(col);

            }
            return col;
        }

        //public static GameObject Instantiate(GameObject prefab)
        //{
        //    return null;
        //}

        public void SetActive(bool bActive)
        {
            this.bActive = bActive;
        }


        public ZServerComponent AddComponent(Type type)
        {
            //new component
            var obj = Activator.CreateInstance(type) as ZServerComponent;

            components.Add(obj);

            obj.gameObject = this;

            return obj;
        }

        public ZServerComponent AddComponent<T>() where T : ZServerComponent
        {
            //new component
            var obj = Activator.CreateInstance(typeof(T)) as ZServerComponent;

            components.Add(obj);

            obj.gameObject = this;

            return obj;
        }



        //public ZServerComponent GetComponent()
        //{
        //    return null;
        //}

        public ZServerComponent GetComponent(Type type)
        {
            var c = components.Find((obj) => type.GetType().IsAssignableFrom(obj.GetType()));

            return c;
            //return default(T);
        }


        public T GetComponent<T>()
        {
            var c = components.Find((obj) => typeof(T).IsAssignableFrom(obj.GetType()));

            return (T)(object)c;
            //return default(T);
        }

        public List<T> GetComponentsInChildren<T>()
        {
            List<T> ret = new List<T>();
            ret.AddRange(GetComponents<T>());

            foreach (var o in Objects)
            {
                ret.AddRange(o.GetComponents<T>());
            }


            return ret;
        }

        public T GetComponentInChildren<T>()
        {
            var list = GetComponentsInChildren<T>();
            return list.FirstOrDefault<T>();
        }

        public List<T> GetComponents<T>()// where T : ZServerComponent
        {
            List<T> ret = new List<T>();

            return components.Where((arg) => typeof(T).IsAssignableFrom(arg.GetType())).Select((arg) => (T)(object)arg).ToList();

        }

        public List<GameObject> GetAllChildren()
        {
            List<GameObject> ret = new List<GameObject>();
            foreach (var o in Objects)
            {
                ret.AddRange(o.GetAllChildren());
            }

            ret.AddRange(GetChildren());

            return ret;
        }

        public List<GameObject> GetChildren() {

            return (Objects as IZPropertyList<GameObject>).ConvertToArray().ToList();
        }

        //GameObject.Find("Monster/Arm/Hand") only support first layer's child
        static public GameObject Find(string name)
        {
            var objs = ZServerScene.Instance.GetAllObjects();
            //objs.Find((GameObject obj) => obj.GetComponent<T>() )
            foreach (var o in objs)
            {
                if (string.Compare(o.name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return o;
            }
            return null;
        }

        public static T FindObjectOfType<T>() where T : MonoBehaviour
        {
            
            var objs =  ZServerScene.Instance.GetAllObjects();
            //objs.Find((GameObject obj) => obj.GetComponent<T>() )
            foreach (var o in objs)
            {
                var c = o.GetComponent<T>();
                if (c != null)
                    return c;

            }
            return default(T);
        }

        internal void Remove(GameObject sub)
        {
            var index = Objects.FindIndex(o => o == sub);
            Objects.RemoveAt(index);
        }

        //remove all children
        internal void RemoveAll()
        {
            Objects.ClearAll();
        }


        static public void Destroy(ZServerObject obj, float t = 0.0F)
        {
            //Observable.ReturnUnit().Delay(TimeSpan.FromSeconds(0.0)).Subscribe
            ObservableEx.NextFrame().Delay(TimeSpan.FromSeconds(t)).Subscribe(_ =>
            {
                var tran = (obj as GameObject)?.transform;
                if (tran != null)
                {
                    tran.parent.gameObject.Remove(tran.gameObject);
                }

                if (Physics.IsInCollision(tran.gameObject))
                {
                    Physics.DelCollision(tran.GetComponent<Collider>());
                }
            });
        }        
        
        static public void DestroyImmediate(ZServerObject obj)
        {
            var tran = (obj as GameObject)?.transform;
            if (tran != null)
            {
                tran.parent.gameObject.Remove(tran.gameObject);
            }

            if (Physics.IsInCollision(tran.gameObject))
            {
                Physics.DelCollision(tran.GetComponent<Collider>());
            }
        }
    }
}

#endif
