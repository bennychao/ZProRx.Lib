

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib
{

    /// <summary>
    /// ZUI text property item.
    /// </summary>
    public class ZUIInputPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        private InputField dataText;

        /// <summary>
        /// Updates the value.
        /// </summary>
        /// <param name="data">Data.</param>
        void IZPropertyViewItem.UpdateValue(object data)
        {
            dataText.text = property.Value.ToString();
        }

        /// <summary>
        /// Bind the specified property.
        /// </summary>
        /// <param name="property">Property.</param>
        bool IZPropertyViewItem.Bind(IZProperty property)
        {

            base.BindBase(property);

            dataText = GetComponent<InputField>();
            if (dataText == null)
                dataText = ZViewBuildTools.FindComponentInChildren<InputField>(this.transform, ZViewCommonObject.Data);

            if (dataText == null)
            {
                Debug.LogError("Can't find Data Component " + property.PropertyID);
                return false;
            }

            dataText.text = property.Value.ToString();

            dataText.onValueChanged.AddListener(OnUIUpdateValue);

            return true;
        }

       new  public void Unbind()
        {
            //do nothing

            dataText.onValueChanged.RemoveListener(OnUIUpdateValue);
        }

        //on ui update
        public void OnUIUpdateValue(string data)
        {
            property.Value = dataText.text;
        }
#endif
    }
}

