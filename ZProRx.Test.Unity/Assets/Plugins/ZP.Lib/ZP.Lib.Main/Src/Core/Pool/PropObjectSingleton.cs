using System;

namespace ZP.Lib
{
	public class PropObjectSingleton<T> : PropObjectSinglenode<T> where T : PropObjectSingleton<T>
	{
		// protected static volatile T _Instance;

        // private static readonly object LockHelper = new object();

        // public static T Instance
		// {
		// 	get
		// 	{
		// 		if (_Instance == null)
		// 		{
        //             //synchronized 
        //             lock (LockHelper)
        //             {
        //                 if (_Instance == null)
        //                 {
        //                     _Instance = ZPropertyMesh.CreateObject<T>();
        //                 }
        //             }
		// 		}
		// 		return _Instance;
		// 	}

        //     set
        //     {
        //         _Instance = value;
        //     }
		// }

        // public static TSub GetInstance<TSub>() where TSub : PropObjectSingleton<T>
        // {
        //     return Instance as TSub;
        // }
	}

	public class PropObjectSinglenode<T>  where T : class
	{
		protected static volatile T _Instance;

        private static readonly object LockHelper = new object();

        public static T Instance
		{
			get
			{
				if (_Instance == null)
				{
                    //synchronized 
                    lock (LockHelper)
                    {
                        if (_Instance == null)
                        {
                            _Instance = ZPropertyMesh.CreateObject<T>();
                        }
                    }
				}
				return _Instance;
			}

            set
            {
                _Instance = value;
            }
		}

        // public static TSub GetInstance<TSub>() where TSub : PropObjectSingleton<T>
        // {
        //     return Instance as TSub;
        // }
	}
}

