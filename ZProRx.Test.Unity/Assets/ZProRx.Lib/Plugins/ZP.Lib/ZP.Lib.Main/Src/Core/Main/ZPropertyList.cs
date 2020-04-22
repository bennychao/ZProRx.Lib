using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZP.Lib
{
	/// <summary>
	/// Z property list.
	/// </summary>
	public class ZPropertyList<T> : ZProperty<T> , IZPropertyList, IZPropertyList<T>, IZProperty, IZProperty<T>
	{
		protected List<ZProperty<T> > propList = new List<ZProperty<T>>();


		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count{
			get{
				return propList.Count;
			}
		}

        public Action<IZProperty> OnAddItem { set; get; }
        public Action<IZProperty, int> OnRemoveItem { set; get; }


        public ZPropertyList()
        {

        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">Value.</param>
        override protected void SetValue(T value){
            //base.SetValue (value);
            throw new Exception("Property List 's Value is invalid " + PropertyID);
        }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		protected override T GetValue ()
		{
            //throw new Exception("Property List 's Value is invalid " + PropertyID);
			return base.GetValue ();
		}

		/// <summary>
		/// Gets or sets the <see cref="ZP.Lib.ZPropertyList`1"/> at the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public T this [int index] {
			set {
				propList [index].Value = value;

				SendChange ();

			}
			get {
				return propList[index].Value;
			}
		}

        //readonly
        public List<IZProperty> PropList { 
            get
            {
                return this.propList.Select(a => a as IZProperty).ToList();
            } 
        }

        /// <summary>
        /// Clone the specified prop.
        /// </summary>
        /// <param name="prop">Property.</param>
        public override void CopyData(IZProperty prop){

            base.CopyData(prop);

			var curProp = prop as ZPropertyList<T>;
			base.data = curProp.data;

			foreach (var p in curProp.propList) {

				ZProperty<T> newProp = new ZProperty<T> ();
				newProp.CopyData (p);

                newProp.AttributeNode = base.attributeNode;

                if (ZPropertyMesh.IsPropertable (p.Value.GetType ())) {

					newProp.Value = (T)ZPropertyMesh.CloneObject ((object)(p.Value));
					propList.Add (newProp);
				} else {
					propList.Add (newProp);
				}
			}
		}


		/// <summary>
		/// Add the specified propData.
		/// </summary>
		/// <param name="propData">Property data.</param>
		void IZPropertyList.Add(object propData){
            Add((T)propData);

			SendChange ();
		}

		/// <summary>
		/// Add the specified propData.
		/// </summary>
		/// <param name="propData">Property data.</param>
		public void Add(T propData)
        {
            var prop = new ZProperty<T> (propData);
			ZPropertyMesh.BindPropertyAttribute (prop, PropertyID);
			propList.Add ((ZProperty<T>)prop);

			SendChange ();

            if (OnAddItem != null)
                OnAddItem(prop);
		}

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="list">List.</param>
        public void AddRange(ZPropertyList<T> list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public void AddRange(IZPropertyList<T> list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public void AddRange(List<T> list)
        {
            if (list == null)
                return;

            foreach (var item in list)
            {
                Add(item);
            }
        }



        //public void Select<ZT>(IZPropertyList<ZT> source, Func<ZT, T> selector)
        //{
        //    source.OnAddItem += (v) =>
        //    {
        //        Add(selector((ZT)v));
        //    };

        //    source.OnRemoveItem += (v, index) =>
        //    {
        //        //Remove(a => a == selector((ZT)v));
        //    };
        //}


        public void RemoveAt(int index)
        {
            var cur = propList[index];
            propList.RemoveAt(index);

            if (OnRemoveItem != null)
                OnRemoveItem(cur, index);
        }


        public void Remove(Predicate<T> comparer)
        {
            //propList.RemoveAll(p=> comparer(p.Value));
            var item = propList.FindIndex(a => comparer(a.Value));
            if (item >= 0)
            {
                RemoveAt(item);
            }

        }

        public void RemoveObject(Predicate<object> comparer)
        {
            //propList.RemoveAll(p=> comparer(p.Value));
            var item = propList.FindIndex(a => comparer(a.Value));
            if (item >= 0)
            {
                RemoveAt(item);
            }

        }

        public void Remove(T value)
        {
            //propList.RemoveAll(p=> comparer(p.Value));
            var item = propList.FindIndex(a => (object)a.Value == (object)value);
            if (item >= 0)
            {
                RemoveAt(item);
            }

        }

        public List<T> RemoveAll(Predicate<T> comparer)
        {
            List<T> rets = new List<T>();
            var ret = propList.FindAll(a => comparer(a.Value));
            if (ret.Count > 0)
            {
                rets = ret.Select(a => a.Value).ToList();

                //return rets;
            }

            //propList.RemoveAll(p => comparer(p.Value));
            for(int i = 0; i < rets.Count; i++)
            {
                RemoveAt(0);
            }

            return rets;
        }


        public void ClearAll()
        {
            for (int i = propList.Count - 1; i >=0; i--)
            {
                RemoveAt(i);
            }

            propList.Clear();
        }

        /// <summary>
        /// Contains the specified t.
        /// </summary>
        /// <param name="t">T.</param>
        public bool Contains(T t){
			return propList.Find (a => 
				{
					return a.Value.Equals(t);
				}) != null;
		}

		/// <summary>
		/// Finds the value.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="Comparer">Comparer.</param>
		public T FindValue(Func<T, bool> Comparer){
			var ret = propList.Find (a => Comparer (a.Value));
            if (ret != null)
                return ret;

            return default(T);
  		
        }

        public List<T> FindValues(Func<T, bool> Comparer)
        {
            List<T> rets = new List<T>();
            var ret = propList.FindAll(a => Comparer(a.Value));
            if (ret.Count > 0)
            {
                rets = ret.Select(a => a.Value).ToList();

                return rets;
            }

            return null;

        }

        /// <summary>
        /// Finds the index.
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="Comparer">Comparer.</param>
        public int FindIndex(Func<T, bool> Comparer)
        {
            return propList.FindIndex(a => Comparer(a.Value));
        }

        /// <summary>
        /// Finds the property.
        /// </summary>
        /// <returns>The property.</returns>
        /// <param name="Comparer">Comparer.</param>
        public IZProperty FindProperty(Func<T, bool> Comparer){
			return propList.Find (a => Comparer (a.Value));
		}

        public float Sum(Func<T, float> selector)
        {
            float ret = 0;

            ret = propList.Sum(a => selector(a.Value));

            return ret;
        }

        public void ActiveChild(int index, bool b)
        {
            propList[index]?.TransNode?.gameObject.SetActive(b);
        }

        public void ActiveAllChild(bool b)
        {
            foreach (var p in propList)
            {
                p.TransNode?.gameObject.SetActive(b);
            }
        }

        public override string ToString()
        {
            return propList.ToString();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator(){
			return propList.GetEnumerator ();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator(){

			var valList = propList.Select(a => a.Value).ToList();
			return ((IEnumerable<T>)valList).GetEnumerator ();
		}

        //public void AddEventHandler()

		/// <summary>
		/// Converts to array.
		/// </summary>
		/// <returns>The to array.</returns>
		object IZPropertyList.ConvertToArray (){
			return propList.Select(a => a.Value).ToList<T>();
		}

        /// <summary>
        /// Converts to array.
        /// </summary>
        /// <returns>The to array.</returns>
        IEnumerable<T> IZPropertyList<T>.ConvertToArray (){
			return propList.Select(a => a.Value).ToList<T>();
		}



    }
}

