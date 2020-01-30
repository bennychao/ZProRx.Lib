using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    public class ZPropertyCache<T> where T : class
    {
        protected static volatile T _instance = default(T);
        public static T Cache
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(ZPropertyCache<T>))
                    {
                        if (_instance == null)
                        {
                            _instance = ZPropertyMesh.CreateObject<T>();
                        }
                    }
                }

                return _instance;
            }

        }
    }

}
