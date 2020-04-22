
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib
{

    /// <summary>
    /// ZUIB ar property item. bind the exp Class
    /// </summary>
    public class ZUIExpPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        private Slider DataBar;
        private Text DescriptionText;

        private IZProperty propBarDataPtr;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        new public bool Bind(IZProperty property)
        {
            base.BindBase(property);

            BindDescription(property, ref DescriptionText, this.transform);

            DataBar = ZViewBuildTools.FindComponentInChildren<Slider>(this.transform, ZViewCommonObject.Bar);

            if (DataBar == null)
            {
                Debug.LogError("Can't find Bar Component " + property.PropertyID);
                return false;
            }

            var propBarData = ZPropertyMesh.GetProperty(property, "ZExp.CurBar");
            if (propBarData == null)
            {
                Debug.LogError("Cur is not set " + property.PropertyID);
                return false;
            }

            var exp = property.Value as ZExp;
            if (exp == null)
            {
                Debug.LogError("property value is not ZExp " + property.PropertyID);
                return false;
            }


            propBarDataPtr = propBarData;

            propBarDataPtr.OnValueChanged += UpdateValue;

            DataBar.value = (float)(propBarDataPtr.Value);

            //DataBar.maxValue = exp.Max;

            return true;
        }

        new public void Unbind()
        {
            //do nothing
            base.UnbindBase();
        }

        new public void UpdateValue(object data)
        {
            //throw new System.NotImplementedException();
            if (propBarDataPtr != null)
                DataBar.value = (float)(propBarDataPtr.Value);

            //DataBar.maxValue = (property.Value as ZExp).Max;
        }
#endif
    }


}

