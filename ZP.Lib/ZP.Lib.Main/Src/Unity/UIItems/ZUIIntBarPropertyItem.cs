
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib
{
    public class ZUIIntBarPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
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

            //BindDescription(property, ref DescriptionText, this.transform);

            DataBar = ZViewBuildTools.FindComponentInChildren<Slider>(this.transform, ZViewCommonObject.Bar);

            if (DataBar == null)
            {
                Debug.LogError("Can't find Bar Component " + property.PropertyID);
                return false;
            }


            if (ZPropertyMesh.IsRuntimable(property))
            {
                var run = property as IRuntimable;
                DataBar.value = (float)run.CurValue / (float)property.Value;

                DescriptionText = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Description);

                if (DescriptionText != null)
                {
                    DescriptionText.text = run.CurValue.ToString() + "/" + property.Value.ToString();
                }

                return true;
            }


            var bar = property.Value as ZIntBar;
            if (bar != null)
            {
                //Debug.LogError("property value is not ZIntBar " + property.PropertyID);
                //return false;

                IZProperty curProp = bar.GetValueProp();
                if (curProp == null)
                {
                    Debug.LogError("Cur is not set " + property.PropertyID);
                    return false;
                }


                curProp.OnValueChanged += UpdateValue;

                this.curProp = curProp;
                // int curData = (int)curProp.Value;
                DataBar.value = (float)(curProp.Value);

                //DataBar.maxValue = bar.Max.Value;

                DescriptionText = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Description);

                if (DescriptionText != null)
                {
                    DescriptionText.text = bar.ToString();
                }

                return true;
            }

            Debug.LogError("property value is not ZIntBar or IRuntimable " + property.PropertyID);





            //DataBar.maxValue = bar.Max.Value;

            return false;
        }

        new public void Unbind()
        {
            //TODO
        }

        public void UpdateValue(object data)
        {

            if (ZPropertyMesh.IsRuntimable(property))
            {
                var run = property as IRuntimable;
                DataBar.value = (float)run.CurValue / (float)property.Value;


                if (DescriptionText != null)
                {
                    DescriptionText.text = run.CurValue.ToString() + "/" + property.Value.ToString();
                }

            }
            else
            {
                DataBar.value = (float)(curProp.Value);

                var bar = property.Value as ZIntBar;
                //DataBar.maxValue = bar.Max.Value;

                if (DescriptionText != null)
                {
                    DescriptionText.text = bar.ToString();
                }
            }


        }

        // Update is called once per frame
        void Update()
        {

        }

#endif
    }

}


