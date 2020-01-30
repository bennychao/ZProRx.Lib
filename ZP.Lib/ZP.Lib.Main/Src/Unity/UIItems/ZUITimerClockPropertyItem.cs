
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib
{
    //support int and ZDataTime Property
    public class ZUITimerClockPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        private Text MinuteText, SecondText;

        public bool Bind(IZProperty property)
        {
            base.BindBase(property);

            MinuteText = GetComponent<Text>();
            if (MinuteText == null)
                MinuteText = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Minute);

            if (MinuteText == null)
            {
                Debug.LogError("Can't find MinuteText Component " + property.PropertyID);
                return false;
            }

            SecondText = GetComponent<Text>();
            if (SecondText == null)
                SecondText = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Second);

            if (SecondText == null)
            {
                Debug.LogError("Can't find SecondText Component " + property.PropertyID);
                return false;
            }

            //MinuteText.text = ((int)property.Value / 60).ToString("D2");
            //SecondText.text = ((int)property.Value % 60).ToString("D2");

            UpdateValue(null);

            return true;
        }

        new public void Unbind()
        {
            //do nothing
        }

        public void UpdateValue(object data)
        {
            if ((property as IZProperty<int>) != null)
            {
                MinuteText.text = ((int)property.Value / 60).ToString("D2");
                SecondText.text = ((int)property.Value % 60).ToString("D2");
            }
            else if ((property as IZProperty<ZDateTime>) != null)
            {
                var dt = ((ZDateTime)property.Value).ToDate();
                MinuteText.text = (dt.Minute).ToString("D2");
                SecondText.text = (dt.Second).ToString("D2");
            }

        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
#endif
    }

}
