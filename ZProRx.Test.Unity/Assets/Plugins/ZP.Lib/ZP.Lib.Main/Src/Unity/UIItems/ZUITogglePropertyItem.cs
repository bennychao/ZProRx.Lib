using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ZP.Lib.Unity
{
    public class ZUITogglePropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        private Toggle toggleData;

        private Text lableText;

        /// <summary>
        /// Updates the value.
        /// </summary>
        /// <param name="data">Data.</param>
        void IZPropertyViewItem.UpdateValue(object data)
        {
            if (lableText != null)
                lableText.text = property.Name;
        }

        /// <summary>
        /// Bind the specified property.
        /// </summary>
        /// <param name="property">Property.</param>
        bool IZPropertyViewItem.Bind(IZProperty property)
        {

            base.BindBase(property);

            toggleData = GetComponent<Toggle>();
            if (toggleData == null)
                toggleData = ZViewBuildTools.FindComponentInChildren<Toggle>(this.transform, ZViewCommonObject.Data);

            if (toggleData == null)
            {
                Debug. LogError("Can't find Data Component " + property.PropertyID);
                return false;
            }

            lableText = toggleData.transform.Find("Label")?.GetComponent<Text>();

            toggleData.isOn = (bool)property.Value;

            lableText.text = property.Name;

            toggleData.onValueChanged.AddListener(OnUIUpdateValue);

            return true;
        }

        new public void Unbind()
        {
            //do nothing

            toggleData.onValueChanged.RemoveListener(OnUIUpdateValue);
        }

        //on ui update
        public void OnUIUpdateValue(bool bSelected)
        {
            property.Value = bSelected;
        }
#endif
    }
}
