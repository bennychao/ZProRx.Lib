using System;

namespace ZP.Lib.Common
{
	public class Cache<T>
	{
		protected static T _instance = default(T);
		public static T  Instance {
			get{
				if (_instance == null) {
					_instance = SinglePool<T>.GetInstance ();
				}

				return _instance;
			}
//			set{
//				if (value != null)
//					_instance = value;
//			}

		}
	}

	public class Temp<T>{
		public static T _instance = default(T);

		public static T Instance{
			get {
				
				if (_instance == null) {
					//_instance = Pool<T>.CreateInstance ();
					throw new SystemException("Not temp");
				}

				return _instance;
			}
			set{
				if (value != null)
					_instance = value;
			}
		}
	}

}

