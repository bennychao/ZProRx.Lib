
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif

namespace ZP.Lib {
	public class ZUIPropertyItemBehaviour : ZPropertyViewItemBehaviour {
#if ZP_UNITY_CLIENT
        private Text nametext;
#endif

		protected override bool BindBase(IZProperty property)
		{
			base.BindBase (property);
#if ZP_UNITY_CLIENT
			nametext = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Name);

			if (nametext == null)
				return false;

			nametext.text = property.Name;
#endif
			return true;
		}


#if ZP_UNITY_CLIENT
        /// <summary>
        /// Binds the description.
        /// </summary>
        static public bool BindDescription(IZProperty property, ref Text ctrlText, Transform parentTran)
        {

            ctrlText = ZViewBuildTools.FindComponentInChildren<Text>(parentTran, ZViewCommonObject.Description);

            if (ctrlText == null)
                return false;

            ctrlText.text = property.Description;

            return true;
        }
#endif
        public void Unbind()
        {
            //do nothing
            base.UnbindBase();
        }

//#if ZP_SERVER
        public bool Bind(IZProperty property)
        {
            return true;
        }
        public void UpdateValue(object data)
        {

        }
//#endif
    }
}

