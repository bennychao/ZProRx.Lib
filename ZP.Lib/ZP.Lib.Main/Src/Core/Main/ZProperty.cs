using System;
using System.Threading;
using UnityEngine;
using UniRx;

namespace ZP.Lib
{
	/// <summary>
	/// Z property.
	/// </summary>
	 public partial class ZProperty<T> : IZProperty, IZProperty<T>
    {
		protected  T data;

        //protected Transform tran;

		protected ZPropertyAttributeNode attributeNode;

        public Transform TransNode { set; get; }

        //public Action<T> OnValueChanged;

        public ZPropertyAttributeNode AttributeNode{ 
			set {
                //only support assign one time
                if (Interlocked.CompareExchange<ZPropertyAttributeNode>(ref attributeNode, value, null) == null)
                //if (attributeNode == null)
                {
                    //attributeNode = value;
                }
                else
                {
                    throw new Exception("AttributeNode only support assign one time!!");
                }
			}
			get{ 
				return attributeNode;
			}
		}

		object IZProperty.Value{
			set{
				this.Value = (T)value;
			}
			get{
				return (object)GetValue();
			}
		}

		public T Value{
			set{
				SetValue (value);
			}
			get{
				return GetValue();
			}
		}



		//bind when build the ui
		public string PropertyID {
			get {
				return attributeNode.PropertyID;
			}
		}

        //ex ".PropertyId"
        public string SimplePropertyID
        {
            get
            {
                return attributeNode.SimplePropertyID;
            }
        }

        public string Name {
			get {
				return attributeNode.Name;
			}
		}

		public string Description {
			get {
				return attributeNode.Description;
			}
		}

        Action<object> _innerValueChangedOld;

        Action<object> IZProperty.OnValueChanged
        {
            set
            {
                _innerValueChangedOld = value;
            }
            get
            {
                return _innerValueChangedOld;
            }
        }

        Action<T> _innerValueChanged;

        public Action<T> OnValueChanged
        {
            set
            {
                _innerValueChanged = value;
            }
            get
            {
                return _innerValueChanged;
            }
        }

        //get frist
        public IZPropertyViewItem ViewItem
        {
            get
            {
                if (TransNode == null)
                    return null;

                return TransNode.GetComponent<IZPropertyViewItem>();
            }

        }

       

        public ZProperty ()
		{
		}


		public ZProperty (T data)
		{
			this.data = data;

            CheckValueChangeAnchor(data);
        }

		/// <summary>
		/// Clone the specified prop.
		/// </summary>
		/// <param name="prop">Property.</param>
		void IZProperty.Copy (IZProperty prop)
		{
			CopyData (prop as ZProperty<T>);
		}

		/// <summary>
		/// Clone the specified prop.
		/// </summary>
		/// <param name="prop">Property.</param>
		public virtual void CopyData(IZProperty prop){
			this.data = (prop as ZProperty<T>).data;
			//this.attributeNode = (prop as ZProperty<T>).attributeNode;
            //this.TransNode = prop.TransNode;

        }

		/// <summary>
		/// Gets the type of the value.
		/// </summary>
		/// <returns>The value type.</returns>
		Type IZProperty.GetDefineType()
		{
			return typeof(T);
		}

		/// <summary>
		/// Gets the type of the value.
		/// </summary>
		/// <returns>The value type.</returns>
		bool IZProperty.IsInstanceOf(Type interfaceType)
		{
			return interfaceType.IsAssignableFrom (typeof(T));
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="value">Value.</param>
		virtual protected void SetValue(T value){
			data = value;

			SendChange ();

            CheckValueChangeAnchor(value);

        }

		protected void SendChange(){
			if (_innerValueChanged != null)
                _innerValueChanged(data);

            if (_innerValueChangedOld != null)
            {
                _innerValueChangedOld(data);
            }

            if (TransNode != null)
            {
                var propItems = TransNode.GetComponents<IZPropertyViewItem>();

                //prop.ViewItem = propItem;
                foreach (var p in propItems)
                {
                    p.UpdateValue((object)data);
                }
            }
        }

        protected void SendChange(T cur)
        {
            if (_innerValueChanged != null)
                _innerValueChanged(cur);

            if (_innerValueChangedOld != null)
            {
                _innerValueChangedOld(cur);
            }

            if (TransNode != null)
            {
                var propItems = TransNode.GetComponents<IZPropertyViewItem>();

                //prop.ViewItem = propItem;
                foreach (var p in propItems)
                {
                    p.UpdateValue((object)cur);
                }

            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value.</returns>
        virtual protected T GetValue(){
			return data;
		}

        public void ActiveNode(bool bEnable = true)
        {
            if (TransNode != null)
            {
                TransNode.gameObject.SetActive(bEnable);
            }
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="ZP.Lib.ZProperty`1"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="ZP.Lib.ZProperty`1"/>.</returns>
        public override string ToString(){
			return data.ToString();
		}

		/// <param name="d">D.</param>
		public static implicit operator T(ZProperty<T> d)  // implicit digit to byte conversion operator
		{
            //if (d == null)
            //{
            //    int i = 0;
            //    i = 100;
            //}

			return d.data;  // implicit conversion
		}

        protected void CheckValueChangeAnchor(T value)
        {
            if (AttributeNode != null && !AttributeNode.IsValueAnchorSupportl())
                return;

            //bind Value Change Anchor
            var anchor = ZPropertyAttributeTools.GetAttribute<PropertyValueChangeAnchorClassAttribute>(typeof(T));
            if (anchor != null)
            {
                foreach (var mid in anchor.MultiPropIds)
                {
                    var subProp = ZPropertyMesh.GetPropertyEx(value, mid);
                    subProp.ValueChangeAsObservable().Subscribe(_ =>
                    {
                        SendChange();
                    });
                }
            }
        }



        //public virtual bool IsBind()
        //{
        //    return ViewItem != null;
        //}

        ///// <summary>
        ///// Ises the bind.
        ///// </summary>
        ///// <returns><c>true</c>, if bind was ised, <c>false</c> otherwise.</returns>
        //bool IZProperty.IsBind()
        //{
        //    return this.IsBind();
        //}

        //
        //		public static implicit operator ZProperty<T>(T d)  // implicit digit to byte conversion operator
        //		{
        //			var newProp = new ZProperty<T>(d);  // implicit conversion
        //			//ZPropertyMgr.BindPropertyAttribute(newProp, ????)
        //			//newProp.AttributeNode = this.attributeNode;
        //
        //			return newProp;
        //		}
    }
}

