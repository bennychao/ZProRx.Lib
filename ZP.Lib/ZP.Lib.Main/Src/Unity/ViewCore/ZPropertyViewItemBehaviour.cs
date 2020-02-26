//#if ZP_UNITY_CLIENT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZP.Lib {
	public class ZPropertyViewItemBehaviour : MonoBehaviour {

		protected IZProperty property;
        protected MultiDisposable disposables = new MultiDisposable();

        volatile protected bool isBind = false;

        public bool IsBind => isBind;

        protected virtual bool BindBase(IZProperty property)
		{
			this.property = property;           

            this.isBind = true;
//			//add Component
//			var attr = property.AttributeNode.GetTypeAttribute<PropertyAddComponentAttribute> ();
//			if (attr != null) {
//				attr.AddComponents (gameObject);
//				return true;
//			}
//
//			attr = property.AttributeNode.GetTypeAttribute<PropertyAddComponentClassAttribute> ();
//			if (attr != null) {
//				attr.AddComponents (gameObject);
//			}

			return true;
		}

        public void UnbindBase()
        {
            disposables.Dispose();
        }

        public IZProperty GetProperty(){
			return property;
		}
	}

}
//#endif