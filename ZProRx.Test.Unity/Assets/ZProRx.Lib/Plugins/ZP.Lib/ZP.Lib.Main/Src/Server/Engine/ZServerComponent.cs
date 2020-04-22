using System;
using System.Collections.Generic;
using ZP.Lib;


#if ZP_SERVER

namespace UnityEngine
{
    public class ZServerComponent : ZServerObject
    {
        protected bool inIsActive = true;

        public string name
        {
            set => gameObject.name = value;
            get => gameObject.name;
        }

        public bool enabled
        {
            set => inIsActive = value;
            get => inIsActive;
        }


        //base
        public GameObject gameObject
        {
            get;
            set;
        }

        public Transform transform
        {
            get => gameObject.transform;
        }

        public ZServerComponent()
        {
        }

        //internal virtual void LoadParams(BindComponent bindComponent)
        //{

        //}

        public void SetActive(bool bActive)
        {
            this.inIsActive = bActive;
        }


        public ZServerComponent AddComponent(Type type)
        {
            return gameObject.AddComponent(type);
        }


        public ZServerComponent GetComponent()
        {
            return null;
        }


        public T GetComponent<T>()// where T : ZServerComponent
        {
            return gameObject.GetComponent<T>();
        }

        public List<T> GetComponentsInChildren<T>()// where T : ZServerComponent
        {

            return gameObject.GetComponentsInChildren<T>();
        }

        public T GetComponentInChildren<T>()// where T : ZServerComponent
        {

            return gameObject.GetComponentInChildren<T>();
        }

        public List<T> GetComponents<T>() //where T : ZServerComponent
        {
            //List<T> ret = new List<T>();
            return gameObject.GetComponents<T>();
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


        static public void Destroy(ZServerObject obj)
        {
            GameObject.Destroy(obj);
        }

        static public void DestroyImmediate(ZServerObject obj)
        {           
            GameObject.Destroy(obj);
        }
    }
}

#endif