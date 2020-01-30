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
        protected Action<T> onEvent;
        protected ZPropertyAttributeNode attributeNode;

        Action IZEvent.OnEvent
        {
            get
            {
                throw new Exception("Event need a param");
            }
            set
            {
                throw new Exception("Event need a param");
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

            //attributeNode.Get
            if (onEvent != null)
                this.onEvent((T)data);
        }

        public void Invoke()
        {
            if (onEvent != null)
                this.onEvent(default(T));
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

