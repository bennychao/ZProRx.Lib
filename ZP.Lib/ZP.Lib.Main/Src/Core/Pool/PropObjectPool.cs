
using System;

namespace ZP.Lib
{ 
    /// <summary>
    /// Property object pool.
    /// </summary>
    public class PropertyObjPool<T>
    {

        static public T CreateInstance()
        {
            if (typeof(T).IsInterface)
                return default(T);

            T obj = (T)Activator.CreateInstance(typeof(T));

            if (!ZPropertyMesh.IsPropertableLowAPI(typeof(T)))
                return obj;

            if (!ZPropertyMesh.IsPropertable(typeof(T)))
            {
                ZPropertyMesh.Build(typeof(T));
            }

            ZPropertyMesh.BindPropertyNodes(obj);

            //ZPropertyMgr.InvokeCreateMethod (obj);

            return obj;
        }

        static public T CreateInstance(params object[] args)
        {
            if (typeof(T).IsInterface)
                return default(T);

            T obj = (T)Activator.CreateInstance(typeof(T), args);

            if (!ZPropertyMesh.IsPropertableLowAPI(typeof(T)))
                return obj;

            if (!ZPropertyMesh.IsPropertable(typeof(T)))
            {
                ZPropertyMesh.Build(typeof(T));
            }

            ZPropertyMesh.BindPropertyNodes(obj);

            //ZPropertyMgr.InvokeCreateMethod (obj);

            return obj;
        }
    }

}