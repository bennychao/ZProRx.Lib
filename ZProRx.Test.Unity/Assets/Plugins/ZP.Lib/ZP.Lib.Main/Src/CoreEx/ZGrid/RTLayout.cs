using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZP.Lib
{
    public class RTLayout : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool Bind(IZProperty property)
        {
             this.BindBase(property);

            var items = ZViewBuildTools.FindComponentInChildren<Transform>(this.transform, ZViewCommonObject.Items);
            if (items == null)
                return false;

            //get items's property
            var list = ZPropertyMesh.GetPropertyEx(property.Value, ".Items") as IZPropertyList;
            if (list == null)
                return false;

            if (list != null)
            {
                list.OnAddItem += chprop =>
                {
                    ZViewBuildTools.BindProperty(chprop as IZProperty, transform);
                };

                //init the dead status
                list.OnRemoveItem += (delProp, index) =>
                {
                    Transform tran = delProp.TransNode;

                    if (tran != null)
                    {
                        Destroy(tran);
                    }
                };
            }


            return true;
        }

        public void Unbind()
        {

        }

        public void UpdateValue(object data)
        {
           // throw new System.NotImplementedException();
        }

    }
}

