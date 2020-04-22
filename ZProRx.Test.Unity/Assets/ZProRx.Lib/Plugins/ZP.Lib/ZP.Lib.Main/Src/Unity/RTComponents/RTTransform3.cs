
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZP.Lib
{
    //support Vector2 ZTransform2 Vector3 ZTramsform3
    public class RTTransform3 : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
        //public bool SetPosition = false;
        public bool SetRotation = false;
        public bool Bind(IZProperty property)
        {
            base.BindBase(property);
            UpdateValue(null);
            return true;
        }

        public void Unbind()
        {
            base.UnbindBase();
        }

        public void UpdateValue(object data)
        {
            //throw new System.NotImplementedException();


            if (property.Value is Vector2)
            {
                Vector2? vec = property.Value as Vector2?;
                transform.position = (vec).ToVector3();
            }
            else if (property.Value is Vector3)
            {
                transform.position = (Vector3)property.Value;
            }
            else if (property.Value is ZTransform)
            {
                transform.position = (property.Value as ZTransform).ToPosition3();
            }
            else if (property.Value is ZTransform3)
            {
                transform.position = (property.Value as ZTransform3).ToPosition3();
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

    public class RTLocalTransform3 : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
        //public bool SetPosition = false;
        public bool SetRotation = false;
        public bool Bind(IZProperty property)
        {
            base.BindBase(property);

            return true;
        }

        public void Unbind()
        {
            base.UnbindBase();
        }

        public void UpdateValue(object data)
        {
            //throw new System.NotImplementedException();


            if (property.Value is Vector2)
            {
                Vector2? vec = property.Value as Vector2?;
                transform.localPosition = (vec).ToVector3();
            }
            else if (property.Value is Vector3)
            {
                transform.localPosition = (Vector3)property.Value;
            }
            else if (property.Value is ZTransform)
            {
                transform.localPosition = (property.Value as ZTransform).ToPosition3();
            }
            else if (property.Value is ZTransform3)
            {
                transform.localPosition = (property.Value as ZTransform3).ToPosition3();
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

