
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZP.Lib;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib.Unity
{
    public class ZUIBarPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {

#if ZP_UNITY_CLIENT
        private Slider DataBar;
        private Text DescriptionText;
        private IZProperty curProp;
        // Use this for initialization
        void Start()
        {

        }


        public bool Bind(IZProperty property)
        {
            base.BindBase(property);

            BindDescription(property, ref DescriptionText, this.transform);

            DataBar = ZViewBuildTools.FindComponentInChildren<Slider>(this.transform, ZViewCommonObject.Bar);

            if (DataBar == null)
            {
                Debug.LogError("Can't find Bar Component with property " + property.PropertyID);
                return false;
            }

            if (ZPropertyMesh.IsRuntimable(property))
            {
                var run = property as IRuntimable;
                DataBar.value = (float)run.CurValue / (float)property.Value;

                if (DescriptionText != null)
                {
                    DescriptionText.text = run.CurValue.ToString() + "/" + property.Value.ToString();
                }

                return true;
            }

            var bar = property.Value as ZDataBar;
            if (bar != null)
            {
                //Debug.LogError("property value is not ZDataBar " + property.PropertyID);
                //return false;
                var curProp = bar.GetValueProp();
                if (curProp == null)
                {
                    Debug.LogError("ZDataBar cur is not set " + property.PropertyID);
                    return false;
                }

                curProp.OnValueChanged += _ =>
                {
                    UpdateValue(_);
                };
                this.curProp = curProp;

                DataBar.value = (float)(curProp.Value);

                if (DescriptionText != null)
                {
                    DescriptionText.text = bar.ToString();
                }

                return true;
            }

            Debug.LogError("property value is not ZDataBar or IRuntimable " + property.PropertyID);

            //DataBar.maxValue = bar.Max.Value;

            return false;
        }

        new public void Unbind()
        {
            //throw new System.NotImplementedException();
            base.UnbindBase();
        }

        public void UpdateValue(object data)
        {
            if (ZPropertyMesh.IsRuntimable(property))
            {
                var run = property as IRuntimable;
                DataBar.value = (float)run.CurValue /(float) property.Value;

                if (DescriptionText != null)
                {
                    DescriptionText.text = run.CurValue.ToString() + "/" + property.Value.ToString();
                }

            }
            else
            {
                DataBar.value = (float)(curProp.Value);

                if (DescriptionText != null)
                {
                    DescriptionText.text = DataBar.ToString();
                }
            }
           

           // DataBar.maxValue = (property.Value as ZDataBar).Max.Value;
        }


        // Update is called once per frame
        void Update()
        {

        }

#endif
    }

}
