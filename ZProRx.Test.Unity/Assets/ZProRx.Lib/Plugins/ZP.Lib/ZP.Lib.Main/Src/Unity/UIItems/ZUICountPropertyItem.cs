
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib
{
    public class ZUICountPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {

#if ZP_UNITY_CLIENT
        private Image icon;
        private Text countText;
        // Use this for initialization
        void Start()
        {

        }

        //Realize the IZPropertyViewItem interface

        new public bool Bind(IZProperty property)
        {
            base.BindBase(property);
            //countText = GetComponent<TextMeshProUGUI>();
            //if (countText == null)
            //    countText = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Data);

            countText = GetComponent<Text>();
            if (countText == null)
                countText = ZViewBuildTools.FindComponentInChildren<Text>(this.transform, ZViewCommonObject.Data);

            if (countText == null)
            {
                Debug.LogError("Can't find Count Component " + property.PropertyID + " transform is  " + transform.name);
                return false;
            }

            countText.text = property.Value.ToString();
            //icon = ZViewBuildTools.FindInChilds(this.transform, ZViewCommonObject.Icon).GetComponent<Image>();

            //if (icon == null)
            //    return false;

            //icon.s

            return true;
        }

        new public void UpdateValue(object data)
        {
            countText.text = property.Value.ToString();
        }


        // Update is called once per frame
        void Update()
        {

        }

#endif
    }
}

