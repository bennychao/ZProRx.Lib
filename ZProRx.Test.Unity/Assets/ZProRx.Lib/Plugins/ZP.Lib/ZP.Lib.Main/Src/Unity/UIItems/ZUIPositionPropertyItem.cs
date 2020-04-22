
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif

namespace ZP.Lib
{
    //support Vector2 / ZTransform
    /// <summary>
    /// ZUI text property item.
    /// </summary>
    public class ZUIPositionPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        private Text dataText;

        /// <summary>
        /// Updates the value.
        /// </summary>
        /// <param name="data">Data.</param>
        void IZPropertyViewItem.UpdateValue(object data)
        {
            if (property.Value is Vector2)
                dataText.text = property.Value.ToString();
            else
                dataText.text = (property.Value as ZTransform)?.ToPosString();
            
        }

        /// <summary>
        /// Bind the specified property.
        /// </summary>
        /// <param name="property">Property.</param>
        bool IZPropertyViewItem.Bind(IZProperty property)
        {
            base.BindBase(property);

            dataText = GetComponent<Text>();
            if (dataText == null)
                dataText = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Data);

            if (dataText == null)
            {
                Debug.LogError("Can't find Data Component " + property.PropertyID);
                return false;
            }

            if (property.Value is Vector2)
                dataText.text = property.Value.ToString();
            else
                dataText.text = (property.Value as ZTransform)?.ToPosString();

            return true;
        }

        //new public void Unbind()
        //{
        //    //do nothing
        //}
#endif
    }
}



