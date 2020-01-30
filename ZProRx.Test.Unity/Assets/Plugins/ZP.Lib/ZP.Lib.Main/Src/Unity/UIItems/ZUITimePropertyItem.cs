
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib
{
    public class ZUITimePropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        public enum TimeType
        {
            Time,
            Date,
            DateAndTime
        }

        public TimeType Type;
        private Text dataText;

        public bool Bind(IZProperty property)
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

            var timer = property.Value as ZDateTime;
            if (timer == null)
            {
                Debug.LogError("It's not the DateTime property " + property.PropertyID);
                return false;
            }

            switch (Type)
            {
                case TimeType.Date:
                    dataText.text = timer.date.ToString();
                    break;

                case TimeType.Time:
                    dataText.text = timer.time.ToString();
                    break;

                case TimeType.DateAndTime:
                    dataText.text = timer.ToString();
                    break;

            }

            return true;
        }

        new public void Unbind()
        {
            //do nothing
        }

        public void UpdateValue(object data)
        {
            var timer = property.Value as ZDateTime;

            switch (Type)
            {
                case TimeType.Date:
                    dataText.text = timer.date.ToString();
                    break;

                case TimeType.Time:
                    dataText.text = timer.time.ToString();
                    break;

                case TimeType.DateAndTime:
                    dataText.text = timer.ToString();
                    break;

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
