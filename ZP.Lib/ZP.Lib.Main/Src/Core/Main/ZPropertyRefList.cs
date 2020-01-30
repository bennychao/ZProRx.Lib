using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace ZP.Lib
{
    /// <summary>
    /// Z property list.
    /// </summary>
    public partial class ZPropertyRefList<T> : ZProperty<T>, IZPropertyRefList, IZPropertyRefList<T>
    {
        private ZPropertyInterfaceRef<T>.RefBindEvent onBindProp = null;
        public ZPropertyInterfaceRef<T>.RefBindEvent OnBindProp
        {
            get
            {
                return onBindProp;
            }

            set
            {
                onBindProp = value;
                //set childs' BindProp
                foreach (var p in propList)
                {
                    p.OnBindProp = onBindProp;
                }
            }

        }

        //Array
        public Action<IZProperty> OnAddItem { set; get; }
        public Action<IZProperty, int> OnRemoveItem { set; get; }


        protected List<ZPropertyInterfaceRef<T>> propList = new List<ZPropertyInterfaceRef<T>>();

        public ZPropertyRefList()
        {
            this.onBindProp = null;
        }

        public ZPropertyRefList(ZPropertyInterfaceRef<T>.RefBindEvent onbind)
        {
            this.onBindProp = onbind;
        }




        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return propList.Count;
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">Value.</param>
        override protected void SetValue(T value)
        {
            //base.SetValue (value);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value.</returns>
        protected override T GetValue()
        {
            return base.GetValue();
        }

        /// <summary>
        /// Gets or sets the <see cref="ZP.Lib.ZPropertyList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">Index.</param>
        public T this[int index]
        {
            set
            {
                propList[index].Value = value;

                SendChange();

            }
            get
            {
                return propList[index].Value;
            }
        }

        /// <summary>
        /// Binds the references.
        /// </summary>
        public void BindRefs()
        {
            foreach (var p in propList)
            {
                p.BindRef();
            }
        }

        public List<IZProperty> PropList
        {
            get
            {
                return this.propList.Select(a => a as IZProperty).ToList();
            }
        }

        /// <summary>
        /// Clone the specified prop.
        /// </summary>
        /// <param name="prop">Property.</param>
        public override void CopyData(IZProperty prop)
        {
            base.CopyData(prop);

            var curProp = prop as ZPropertyRefList<T>;
            base.data = curProp.data;

            this.onBindProp = curProp.onBindProp;

            foreach (var p in curProp.propList)
            {
                ZPropertyInterfaceRef<T> newProp = new ZPropertyInterfaceRef<T>();
                newProp.CopyData(p);

                if (ZPropertyMesh.IsPropertable(p.Value.GetType()))
                {
                    newProp.Value = (T)ZPropertyMesh.CloneObject((object)(p.Value));
                    propList.Add(newProp);
                }
                else
                {
                    propList.Add(newProp);
                }
            }
        }



        /// <summary>
        /// Add the specified propData.
        /// </summary>
        /// <param name="propData">Property data.</param>
        void IZPropertyRefList.Add(int refid)
        {
            // Add((T)propData);
            Add(refid);

            SendChange();
        }

        /// <summary>
        /// Add the specified propData.
        /// </summary>
        /// <param name="propData">Property data.</param>
        public void Add(int refid)
        {
            var prop = new ZPropertyInterfaceRef<T>(refid, OnBindProp);

            ZPropertyMesh.BindPropertyAttribute(prop, PropertyID);
            propList.Add((ZPropertyInterfaceRef<T>)prop);

            if (OnAddItem != null)
                OnAddItem(prop);

            SendChange();
        }

        public void Add(int id, T value)
        {
            var prop = new ZPropertyInterfaceRef<T>(id, OnBindProp);

            prop.Value = value;

            ZPropertyMesh.BindPropertyAttribute(prop, PropertyID);
            propList.Add((ZPropertyInterfaceRef<T>)prop);

            if (OnAddItem != null)
                OnAddItem(prop);

            SendChange();
        }

        public void Add(ZPropertyInterfaceRef<T> rprop)
        {
            var prop = new ZPropertyInterfaceRef<T>(rprop.RefID, rprop.OnBindProp);

            prop.Value = rprop.Value;

            ZPropertyMesh.BindPropertyAttribute(prop, PropertyID);
            propList.Add((ZPropertyInterfaceRef<T>)prop);

            if (OnAddItem != null)
                OnAddItem(prop);

            SendChange();
        }

        public void Add(IIndexable indexer)
        {
            if (indexer == null)
                return;

            Add(indexer.Index, (T)indexer);
        }

        public void AddRange(IZPropertyRefList list)
        {
            foreach (var p in list)
            {
                Add(p as ZPropertyInterfaceRef<T>);
            }
        }

        //public void AddRange(IZPropertyList<T> list)
        //{
        //    foreach (var p in list)
        //    {
        //        Add(p as ZPropertyInterfaceRef<T>);
        //    }
        //}

        public void AddRange(List<T> list)
        {
            foreach (var p in list)
            {
                Add(p as IIndexable);
            }
        }


        public void RemoveAt(int index)
        {
            var cur = propList[index];
            propList.RemoveAt(index);

            if (OnRemoveItem != null)
                OnRemoveItem(cur, index);
        }

        public void Remove(Predicate<T> comparer)
        {
            var item = propList.FindIndex(a => comparer(a.Value));
            if (item >= 0)
            {
                RemoveAt(item);
            }
        }

        public void RemoveObject(Predicate<object> comparer)
        {
            var item = propList.FindIndex(a => comparer(a.Value));
            if (item >= 0)
            {
                RemoveAt(item);
            }
        }

        public void Remove(Predicate<ZPropertyInterfaceRef<T>> comparer)
        {
            var item = propList.FindIndex(a => comparer(a));
            if (item >= 0)
            {
                RemoveAt(item);
            }
        }

        public void ClearAll()
        {
            for (int i = propList.Count - 1; i >= 0; i--)
            {
                RemoveAt(i);
            }

            propList.Clear();
        }

        /// <summary>
        /// Contains the specified t.
        /// </summary>
        /// <param name="t">T.</param>
        public bool Contains(T t)
        {
            return propList.Find(a =>
            {
                return a.Value.Equals(t);
            }) != null;
        }

        /// <summary>
        /// Contains the specified refid.
        /// </summary>
        /// <returns>The contains.</returns>
        /// <param name="refid">Refid.</param>
        public bool Contains(int refid)
        {
            return propList.Find(a =>
            {
                return a.RefID == refid;
            }) != null;
        }

        public T GetValue(int refid)
        {
            var ret = propList.Find(a => a.RefID == refid);
            if (ret == null)
            {
                return default(T);
            }

            return ret;
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
        /// Finds the value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="Comparer">Comparer.</param>
        public T FindValue(Func<T, bool> Comparer)
        {
            return propList.Find(a => Comparer(a.Value)) ?? default(T);
        }

        /// <summary>
        /// Finds the value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="Comparer">Comparer.</param>
        public T FindValue(Func<int, bool> Comparer)
        {
            var ret = propList.Find(a => Comparer(a.RefID));
            if (ret == null)
            {
                return default(T);
            }

            return ret;
        }

        /// <summary>
        /// Finds the property.
        /// </summary>
        /// <returns>The property.</returns>
        /// <param name="Comparer">Comparer.</param>
        public IZProperty FindProperty(Func<T, bool> Comparer)
        {
            return propList.Find(a => Comparer(a.Value));
        }

        public IZProperty FindProperty(Func<int, bool> Comparer)
        {
            return propList.Find(a => Comparer(a.RefID));
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return propList.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {

            var valList = propList.Select(a => a.Value).ToList();
            return ((IEnumerable<T>)valList).GetEnumerator();
        }

        /// <summary>
        /// Converts to array.
        /// </summary>
        /// <returns>The to array.</returns>
        object IZPropertyRefList.ConvertToArray()
        {
            return propList.Select(a => a.Value).ToList<T>();
        }

        /// <summary>
        /// Converts to array.
        /// </summary>
        /// <returns>The to array.</returns>
        IList<T> IZPropertyRefList<T>.ConvertToArray()
        {
            return propList.Select(a => a.Value).ToList<T>();
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

        public float Sum(Func<T, float> selector)
        {
            float ret = 0;

            ret = propList.Sum(a => selector(a.Value));

            return ret;
        }
    }
}

