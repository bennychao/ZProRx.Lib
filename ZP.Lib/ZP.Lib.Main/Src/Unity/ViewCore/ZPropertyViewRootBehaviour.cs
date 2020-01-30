//#if ZP_UNITY_CLIENT
using System;
using UnityEngine;
using UniRx;

namespace ZP.Lib
{
	public class ZPropertyViewRootBehaviour : MonoBehaviour
	{
		protected object rootObj;

        public IObservable<Unit> OnEnableObservable => onEnableObservable;
        public IObservable<Unit> OnDisableObservable => onDisableObservable;

        protected Subject<Unit> onEnableObservable = new Subject<Unit>();
        protected Subject<Unit> onDisableObservable = new Subject<Unit>();

        public ZPropertyViewRootBehaviour ()
		{
		}

		protected virtual bool BindBase(object rootObj)
		{
			this.rootObj = rootObj;

            //auto find transform items

			return true;
		}

        public void Unbind()
        {

        }

        public bool IsBinded(object obj)
        {
            return rootObj != null && rootObj == obj;
        }

        public void OnEnabled()
        {
            onEnableObservable.OnNext(Unit.Default);
        }

        public void OnDisabled()
        {
            onDisableObservable.OnNext(Unit.Default);
        }

        public object GetRoot(){
			return rootObj;
		}
	}
}
//#endif
