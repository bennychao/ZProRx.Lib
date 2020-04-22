using System;
using UnityEngine;

namespace ZP.Lib.Common
{

	public class GameObjectPool : Pool<GameObject>{
		public GameObject Template {
			get;
			set;
		}

		protected override GameObject CreateInstance(){
			var obj = GameObject.Instantiate(Template);
			obj.SetActive (false);
			return obj;
		}

		public new void ReleaseInstance(GameObject obj){
			base.ReleaseInstance (obj);
			obj.SetActive (false);
		}

		public new GameObject GetInstance()
		{
			var obj = base.GetInstance ();
			obj.SetActive (true);
			return obj;
		}
	}
}

