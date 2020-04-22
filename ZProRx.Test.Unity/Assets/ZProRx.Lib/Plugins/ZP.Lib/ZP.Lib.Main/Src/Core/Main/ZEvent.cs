using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZP.Lib
{
    public static class EventMethod{
        static public string OnClick = "OnClick";
    }


   // public class ZEvent

    public class ZEvent : IZEvent
    {
        protected Action onEvent;
        protected ZPropertyAttributeNode attributeNode;

        public Action OnEvent
        {
            get
            {
                return onEvent;
            }
            set
            {
                onEvent = value;
            }
        }

        public Transform TransNode { set; get; }

        public ZPropertyAttributeNode AttributeNode
        {
            set
            {
                attributeNode = value;
            }
            get
            {
                return attributeNode;
            }
        }

        //bind when build the ui
        public string PropertyID
        {
            get
            {
                return attributeNode.PropertyID;
            }
        }

        public string SimplePropertyID
        {
            get
            {
                return attributeNode.SimplePropertyID;
            }
        }

        virtual public void Invoke()
        {
            if (onEvent != null)
                this.onEvent();
        }

        public void ActiveNode(bool bEnable = true)
        {
            if (TransNode != null)
            {
                TransNode.gameObject.SetActive(bEnable);
            }
        }
    }


    public class ZEvent<T> : IZEvent<T>
    {
        protected T curValue = default(T);

        protected Action<T> onEvent;

        protected Action onEventWithNoParam;

        protected ZPropertyAttributeNode attributeNode;

        public T CurValue => curValue;
        object IZEventWithParam.CurValue => curValue;

        Action IZEvent.OnEvent
        {
            get
            {
                return onEventWithNoParam;
            }
            set
            {
                onEventWithNoParam = value;
            }
        }

        public Action<T> OnEvent
        {
            get
            {
                return onEvent;
            }
            set
            {
                onEvent = value;
            }
        }

        public Transform TransNode { set; get; }

        public ZPropertyAttributeNode AttributeNode
        {
            set
            {
                attributeNode = value;
            }
            get
            {
                return attributeNode;
            }
        }

        public string SimplePropertyID
        {
            get
            {
                return attributeNode.SimplePropertyID;
            }
        }


        //bind when build the ui
        public string PropertyID
        {
            get
            {
                return attributeNode.PropertyID;
            }
        }

        virtual public void Invoke(T data)
        {
            curValue = data;

            //attributeNode.Get
            if (onEvent != null)
                this.onEvent((T)data);


            if (onEventWithNoParam != null)
            {
                this.onEventWithNoParam();
            }
        }

        public void Invoke()
        {
            if (onEvent != null)
                this.onEvent(default(T));

            if (onEventWithNoParam != null)
            {
                this.onEventWithNoParam();
            }
        }

        public void ActiveNode(bool bEnable = true)
        {
            if (TransNode != null)
            {
                TransNode.gameObject.SetActive(bEnable);
            }
        }
    }
}

