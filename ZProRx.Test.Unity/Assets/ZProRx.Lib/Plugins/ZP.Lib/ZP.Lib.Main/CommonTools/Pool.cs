using System;
using System.Collections.Generic;


namespace ZP.Lib.Common
{
	public static class SinglePool<T>
	{
		private static Pool<T> _pool;

		static SinglePool(){
			_pool = new Pool<T> ();
		}

		static public T GetInstance()
		{
			return _pool.GetInstance ();
		}

		static public void ReleaseInstance(T obj){
			_pool.ReleaseInstance (obj);
		}
	}

	public class Pool<T>{
		List<T> liveObjs = new List<T>();
		List<T> deadObjs = new List<T>();
//		static public T CreateSingleInstance()
//		{
//			return (T)Activator.CreateInstance (typeof(T).GetType());
//		}

		protected virtual T CreateInstance(){
			var obj = (T)Activator.CreateInstance (typeof(T).GetType ());
			return obj;
		}

		public T GetInstance()
		{
			if (deadObjs.Count <= 0) {
				var obj = CreateInstance ();
				liveObjs.Add (obj);
				return obj;
			} else {

				var obj = deadObjs [0];
				deadObjs.RemoveAt (0);
				liveObjs.Add (obj);
				return obj;
			}
		}

		public void ReleaseInstance(T obj){
			liveObjs.Remove (obj);
			deadObjs.Add (obj);
		}


		public void AddInstance(T obj){
			liveObjs.Add (obj);
		}

		public void CreateCapacity(int iCount){
			for (int i = 0; i < iCount; i++) {
				var obj = CreateInstance ();
				deadObjs.Add (obj);
			}
		}
	}
}

