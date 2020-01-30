using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace ZP.Lib
{
    /// <summary>
    /// Link Property
    /// </summary>
    /// <Attribute>PropertyLink PropertyLinkSync</Attribute>
    /// <typeparam name="T"></typeparam>
    public class ZLinkProperty<T> : ZProperty<T>, IZLinkable
    {
        protected MultiDisposable disposables = new MultiDisposable();

        public ZLinkProperty() :base()
        {

        }

        public ZLinkProperty(T v) : base(v)
        {

        }

        private IZProperty linkedProp;
        public IZProperty LinkProperty
        {
            get
            {
                return linkedProp;
            }

            set
            {
                SetLink(value);
            }
        }

        protected virtual void SetLink(IZProperty value)
        {
            if (value == null)
                return;

            disposables.Dispose();

            linkedProp = value;

            var attr = AttributeNode.GetAttribute<PropertyLinkSyncAttribute>();
            if (attr != null && attr.bSupport)
            {
                //check type
                if (LinkProperty.GetDefineType() != typeof(T))
                {
                    throw new System.Exception("Link error the type if not same");
                }

                //set current value
                if (ZPropertyMesh.IsRuntimable(linkedProp))
                {

                    base.SetValue((T)(linkedProp as IRuntimable).CurValue);
                }
                else
                {
                    //use base set value not send sync 
                    base.SetValue((T)LinkProperty.Value);
                }

                LinkProperty.ValueChangeAsObservable().Subscribe( v =>
                {
                    //if it is runtime v is cur value
                    base.SetValue((T)v);
                }).AddTo(disposables);
            }
        }


        override protected void SetValue(T value)
        {
            base.SetValue(value);

            if (AttributeNode == null)
                return; 

            var attr =AttributeNode.GetAttribute<PropertyLinkSyncAttribute>();
            if (attr != null && attr.bSupport && LinkProperty != null)
            {
                LinkProperty.Value = value;
            }
        }


        override protected T GetValue()
        {
            //var attr = AttributeNode.GetAttribute<PropertyLinkSyncAttribute>();
            //if (attr != null && attr.bSupport && LinkProperty != null)
            //{
            //    return (T)LinkProperty.Value;
            //}

            return base.GetValue();
        }

        public override void CopyData(IZProperty prop)
        {
            var ip = prop as IZLinkable;
            if (ip == null)
                return;
            linkedProp = ip.LinkProperty; //need to re link 
            //refID = ip.RefID;
            base.CopyData(prop);
        }


    }
}

