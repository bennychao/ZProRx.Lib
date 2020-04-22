using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace ZP.Lib.Unity
{
    //only support ZintBar
    public class ZUITimerProgressPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        private Image progressPointer;
        private Text curData;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        bool IZPropertyViewItem.Bind(IZProperty property)
        {
            base.BindBase(property);
            progressPointer = ZViewBuildTools.FindComponentInChildren<Image>(this.transform, "Progress");
                       
            if (progressPointer == null)
            {
                Debug.LogError("Can't find Data Component " + property.PropertyID);
                return false;
            }

            curData = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Data);

            progressPointer.fillAmount = 1 - (property.Value as ZIntBar).GetValue();

            if (curData != null)
                curData.text = (property.Value as ZIntBar).Cur.Value.ToString();

            return true;
        }

        /// <summary>
        /// Updates the value.
        /// </summary>
        /// <param name="data">Data.</param>
        void IZPropertyViewItem.UpdateValue(object data)
        {
            progressPointer.fillAmount =1 - (property.Value as ZIntBar).GetValue();

            if (curData != null)
                curData.text = (property.Value as ZIntBar).Cur.Value.ToString();
        }

#endif
    }
}

