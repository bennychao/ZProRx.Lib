using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZP.Lib.Unity.RTComponents;

namespace ZP.Lib.Unity.ViewCore
{
    //support bind the event or property
    public class ZPropertyTriggerItemBehaviour : ZPropertyViewItemBehaviour
    {
        [HideInInspector]
        public string SubEventID;

        protected IZEvent zEvent;
        protected  void BindBase(IZEvent e)
        {
            zEvent = e;
        }

        public bool Bind(IZEvent zEvent)
        {
            BindBase(zEvent);

            return true;
        }

        public bool Bind(IZProperty property)
        {
            //throw new NotImplementedException();
            base.BindBase(property);

            var attr = ZPropertyAttributeTools.GetAttribute<RTAddTriggerComponentAttribute>(property);
            if (attr != null)
            {
                SubEventID = attr.SubEventId;
            }
            else
            {
               var  attrsClass = ZPropertyAttributeTools.GetAttributes<RTAddTriggerComponentClassAttribute>(property);

                foreach(var attr1 in attrsClass)
                {
                    if (attr1.IsDefineType(this.GetType()))
                        SubEventID = attr1?.SubEventId;
                }
                
            }
            var zEvent = ZPropertyMesh.GetEventEx(property.Value, SubEventID);

            Bind(zEvent);

            return true;
        }

        //not support update value
        public void UpdateValue(object data)
        {
            //throw new NotImplementedException();
        }
    }
    public class ZPropertyTriggerItemBehaviour<T> : ZPropertyTriggerItemBehaviour
    {
        protected IZEvent<T> Event { get => zEvent as IZEvent<T>; }
    }

}
