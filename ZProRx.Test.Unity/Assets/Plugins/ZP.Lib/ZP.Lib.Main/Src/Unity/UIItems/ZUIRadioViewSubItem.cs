using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZP.Lib.Unity
{
    //highlight Radio 
    public class ZUIRadioViewSubItem : ZUIPropertyItemBehaviour, IZPropertyViewItem, IZRadioViewItem
    {
        private Transform highLightNode;
        public bool Bind(IZProperty property)
        {
            highLightNode = ZViewBuildTools.FindInChilds(transform, ZViewCommonObject.HighLight);

            return base.BindBase(property);
        }

        public void OnSelected()
        {
            highLightNode?.gameObject.SetActive(true);
        }

        public void OnUnselected()
        {
            highLightNode?.gameObject.SetActive(false);
        }

        public void UpdateValue(object data)
        {
            //throw new NotImplementedException();
        }
    }
}
