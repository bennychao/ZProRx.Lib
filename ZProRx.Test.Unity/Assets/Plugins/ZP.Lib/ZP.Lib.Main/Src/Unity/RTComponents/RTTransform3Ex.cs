
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ZP.Lib
{
    //support sub property Vector2 ZTransform2 Vector3 ZTramsform3
    public class RTTransform3Ex : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
        [HideInInspector]
        public string SubMultiPropID;
        [HideInInspector]
        public Vector2 AxisScale = Vector2.one;
        //public bool SetPosition = false;
        //[HideInInspector]
        public bool SetRotation = false;

        IZProperty subProperty = null;
        public bool Bind(IZProperty property)
        {
            base.BindBase(property);

            subProperty = ZPropertyMesh.GetPropertyEx(property.Value, SubMultiPropID);

            if (subProperty != null)
            {
                //update value now
                checkValueChange();

                ZPropertyObservable.ValueChangeAsObservable(subProperty)
                .Subscribe(_=> checkValueChange());
            }

            return true;
        }

        public void Unbind()
        {
            //throw new System.NotImplementedException();
        }

        public void UpdateValue(object data)
        {
            //throw new System.NotImplementedException();


        }

        void checkValueChange()
        {
            if (subProperty.Value is Vector2)
            {
                Vector2? vec = subProperty.Value as Vector2?;

                transform.position = (vec).ToVector3(AxisScale);
            }
            else if (subProperty.Value is Vector3)
            {
                transform.position = (Vector3)subProperty.Value;
            }
            else if (subProperty.Value is ZTransform)
            {
                transform.position = (subProperty.Value as ZTransform).ToAxisPosition3();
            }
            else if (subProperty.Value is ZTransform3)
            {
                transform.position = (subProperty.Value as ZTransform3).ToAxisPosition3();
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class RTLocalTransform3Ex : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
        [HideInInspector]
        public string SubMultiPropID;
        //public bool SetPosition = false;
        //[HideInInspector]
        public bool SetRotation = false;
        [HideInInspector]
        public Vector2 AxisScale = Vector2.one;
        IZProperty subProperty = null;
        public bool Bind(IZProperty property)
        {
            base.BindBase(property);

            subProperty = ZPropertyMesh.GetPropertyEx(property.Value, SubMultiPropID);

            checkValueChange();
            if (subProperty != null)
            {
                ZPropertyObservable.ValueChangeAsObservable(subProperty)
                .Subscribe(_ => checkValueChange());
            }

            return true;
        }

        public void Unbind()
        {
            //throw new System.NotImplementedException();
        }

        public void UpdateValue(object data)
        {
            //throw new System.NotImplementedException();


        }

        void checkValueChange()
        {
            if (subProperty.Value is Vector2)
            {
                Vector2? vec = subProperty.Value as Vector2?;
                transform.localPosition = (vec).ToVector3(AxisScale);
            }
            else if (subProperty.Value is Vector3)
            {
                transform.localPosition = (Vector3)subProperty.Value;
            }
            else if (subProperty.Value is ZTransform)
            {
                transform.localPosition = (subProperty.Value as ZTransform).ToAxisPosition3();
            }
            else if (subProperty.Value is ZTransform3)
            {
                transform.localPosition = (subProperty.Value as ZTransform3).ToAxisPosition3();
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

