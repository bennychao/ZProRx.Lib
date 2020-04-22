using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZP.Lib
{

    /// <summary>
    /// Z property Ex.
    /// </summary>
    public partial class ZProperty<T> : IZProperty, IZProperty<T>
    {

        /// <summary>
        /// Select the specified selector.
        /// </summary>
        /// <param name="selector">Selector.</param>
        public IDisposable Select<ZT>(ZT source, Func<ZT, T> selector) where ZT : IZProperty
        {
            SetValue(selector(source));

            //source.OnValueChanged += (v) => {
            //    SetValue(selector(source));
            //};

            var disposable = new OnValueChangedDisposable(source as IZProperty, (v) =>
            {
                SetValue(selector(source));
            });

            return disposable;

        }

        /// <summary>
        /// Select the specified source, source2 and selector.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <param name="source2">Source2.</param>
        /// <param name="selector">Selector.</param>
        /// <typeparam name="ZT">The 1st type parameter.</typeparam>
        /// <typeparam name="ZTT">The 2nd type parameter.</typeparam>
            public IDisposable Select<ZT, ZTT>(ZT source, ZTT source2, Func<ZT, ZTT, T> selector)
            where ZT : IZProperty
            where ZTT : IZProperty
        {
            SetValue(selector(source, source2));

            //source.OnValueChanged += (v) => {
            //    SetValue(selector(source, source2));
            //};

            //source2.OnValueChanged += (v) => {
            //    SetValue(selector(source, source2));
            //};

            var multiDisposable = new MultiDisposable();

            new OnValueChangedDisposable(source as IZProperty, (v) =>
            {
                SetValue(selector(source, source2));
            }).AddToMultiDisposable(multiDisposable);

            new OnValueChangedDisposable(source2 as IZProperty, (v) =>
            {
                SetValue(selector(source, source2));
            }).AddToMultiDisposable(multiDisposable);


            return multiDisposable as IDisposable;
        }

        //the value will changed follow the list item's change
        public IDisposable Count<ZT>(IZPropertyList<ZT> list)
        {
            this.Value = (T)(object)list.Count;

            //list.OnAddItem += _ => this.Value = (T)(object)list.Count;

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)list.Count;


            var  disposable = new ListDisposable(list as IZPropertyList, 
                _ => this.Value = (T)(object)list.Count,
                (__, ____) => this.Value = (T)(object)list.Count);
            return disposable;
        }



        public IDisposable Count<ZT>(IZPropertyList<ZT> list, Func<ZT, bool> selector)
        {
            this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count;

            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count;

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count;


            var disposable = new ListDisposable(list as IZPropertyList,
                 _ => this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count,
                (_, ___) => this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count
                 );
            return disposable;
        }


        public IDisposable Count<ZT>(IZPropertyRefList<ZT> list)
        {
            this.Value = (T)(object)list.Count;

            //list.OnAddItem += _ => this.Value = (T)(object)list.Count;

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)list.Count;

            var disposable = new RefListDisposable(list as IZPropertyRefList,
                _ => this.Value = (T)(object)list.Count,
                (_, ___) => this.Value = (T)(object)list.Count
                 );
            return disposable;
        }



        public IDisposable Count<ZT>(IZPropertyRefList<ZT> list, Func<ZT, bool> selector)
        {
            this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count;

            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count;

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count;


            var disposable = new RefListDisposable(list as IZPropertyRefList,
                _ => this.Value = this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count,
                (_, ___) => this.Value = (T)(object)(list).Where(a => selector(a)).ToList().Count
                 );
            return disposable;
        }


        //-----SUM View--------------

        private void listItemValueChanged(List<IZProperty> list, Action<object> op)
        {
            foreach (var p in list)
            {
                p.OnValueChanged += op;
            }
        }
        
        private IDisposable listItemValueChangedSafe(List<IZProperty> list, Action<object> op)
        {
            var multiDisposable = new MultiDisposable();
            foreach (var p in list)
            {

                new OnValueChangedDisposable(p as IZProperty, op).AddToMultiDisposable(multiDisposable);
                //p.OnValueChanged += op;
            }

            return multiDisposable;
        }

        private T innerSum(List<IZProperty> list)
        {

            if (ZPropertyMesh.IsCalculable<T>(typeof(T)))
            {
                T sum = ZPropertyMesh.CreateObject<T>();


                foreach (var p in list)
                {
                    var alu = p.Value as ICalculable<T>;

                    if (ZPropertyMesh.IsRuntimable(p))
                        sum = (sum as ICalculable<T>).Add((T)(p as IRuntimable).CurValue);
                    else
                        sum = (sum as ICalculable<T>).Add((T)p.Value);

                }

                return sum;
            }
            else
            {
                double sum = 0;

                foreach (var p in list)
                {

                    if (ZPropertyMesh.IsRuntimable(p))
                        sum += Convert.ToDouble((object)((p as IRuntimable).CurValue));
                    else
                        sum += Convert.ToDouble((object)(p.Value));
                }

                if (typeof(T) == typeof(int))
                    return (T)(object)Convert.ToInt32(sum);
                else if (typeof(T) == typeof(uint))
                    return (T)(object)Convert.ToUInt32(sum);
                else if (typeof(T) == typeof(float))
                    return (T)(object)Convert.ToSingle(sum);
                else if (typeof(T) == typeof(double))
                    return (T)(object)Convert.ToDouble(sum);
                else
                    return (T)(object)sum;
            }

        }




        public IDisposable Sum(IZPropertyList list, string multiPropID){

            //check T is valuetype
            //if (!typeof(T).IsValueType)
            //{
            //    throw new Exception("Sum is only support value type int and float");
            //}

            var plist = ZPropertyMesh.GetPropertiesEx(list, multiPropID);
            this.Value = (T)(object)innerSum(plist);

            listItemValueChanged(plist, _ => this.Value = (T)(object)innerSum(plist));

            //resum the list

            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)innerSum(plist);

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)innerSum(plist);

            var disposable = new ListDisposable(list,
                newItem =>
                {
                    var v = ZPropertyMesh.GetPropertyEx(newItem, multiPropID);
                    plist.Add(v);
                    this.Value = this.Value = (T)(object)innerSum(plist);
                }
                ,
                 (_, ___) => {
                     plist = ZPropertyMesh.GetPropertiesEx(list, multiPropID);
                     this.Value = (T)(object)innerSum(plist);
                 });

            return disposable;
        }

        public IDisposable Sum(IZPropertyRefList list, string multiPropID)
        {

            //check T is valuetype
            //if (!typeof(T).IsValueType)
            //{
            //    throw new Exception("Sum is only support value type int and float");
            //}

            var plist = ZPropertyMesh.GetPropertiesEx(list, multiPropID);
            this.Value = (T)(object)innerSum(plist);

            listItemValueChanged(plist, _ => this.Value = (T)(object)innerSum(plist));

            //resum the list
            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)innerSum(plist);

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)innerSum(plist);

            var disposable = new RefListDisposable(list,
                     newItem =>
                     {
                         var v = ZPropertyMesh.GetPropertyEx(newItem, multiPropID);
                         plist.Add(v);
                         this.Value = this.Value = (T)(object)innerSum(plist);
                     },
                     (_, ___) =>
                     {
                         plist = ZPropertyMesh.GetPropertiesEx(list, multiPropID);
                         this.Value = (T)(object)innerSum(plist);
                     });

            return disposable;
        }


        //support item selector
        public IDisposable Sum<IT>(IZPropertyList<IT> list, string multiPropID, Func<IT, bool> itemSelector)
        {

            //check T is valuetype
            if (!typeof(T).IsValueType)
            {
                throw new Exception("Sum is only support value type int and float");
            }

            var supList = ZPropertyMesh.GetPropertiesEx<IT>(list, multiPropID, itemSelector);
           // var supList = plist.FindAll(a => itemSelector((IT)a.Value));

            this.Value = (T)(object)innerSum(supList);

            listItemValueChanged(supList, _ => this.Value = (T)(object)innerSum(supList));

            //resum the list


            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)innerSum(supList);

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)innerSum(supList);

            var disposable = new ListDisposable(list as IZPropertyList,
                 newItem =>
                 {
                     var v = ZPropertyMesh.GetPropertyEx(newItem, multiPropID);
                     supList.Add(v);
                     this.Value = this.Value = (T)(object)innerSum(supList);
                 },
                  (_, ___) =>
                  {
                      supList = ZPropertyMesh.GetPropertiesEx<IT>(list, multiPropID, itemSelector);
                      this.Value = (T)(object)innerSum(supList);
                  });

            return disposable;

        }

        public IDisposable Sum<IT>(IZPropertyRefList<IT> list, string multiPropID, Func<IT, bool> itemSelector)
        {

            //check T is valuetype
            if (!typeof(T).IsValueType)
            {
                throw new Exception("Sum is only support value type int and float");
            }

            var supList = ZPropertyMesh.GetPropertiesEx<IT>(list, multiPropID, itemSelector);
            // var supList = plist.FindAll(a => itemSelector((IT)a.Value));

            this.Value = (T)(object)innerSum(supList);

            listItemValueChanged(supList, _ => this.Value = (T)(object)innerSum(supList));

            //resum the list

            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)innerSum(supList);

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)innerSum(supList);

            var disposable = new ListDisposable(list as IZPropertyList,
                 newItem =>
                 {
                     var v = ZPropertyMesh.GetPropertyEx(newItem, multiPropID);
                     supList.Add(v);
                     this.Value = this.Value = (T)(object)innerSum(supList);
                 },
                  (_, ___) =>
                  {
                      supList = ZPropertyMesh.GetPropertiesEx<IT>(list, multiPropID, itemSelector);
                      this.Value = (T)(object)innerSum(supList);
                  }
                  );

            return disposable;
        }


        //public IDisposable Sum<IT>(object obj, string multiPropID, Func<IT, bool> itemSelector)
        //{
        //    var supList = ZPropertyMesh.GetPropertiesEx<IT>(obj, multiPropID, itemSelector);

        //    return ;
        //}

        //public IDisposable Sum(object obj, string multiPropID)
        //{
        //    var supList = ZPropertyMesh.GetPropertiesEx(obj, multiPropID);

        //    return null;
        //}

            //support value select
        private T innerSum<ZT>(List<IZProperty> list, Func<ZT, T> selector)
        {

            if (typeof(T) is ICalculable<T>)
            {
                T sum = ZPropertyMesh.CreateObject<T>();


                foreach (var p in list)
                {
                    var alu = p.Value as ICalculable<T>;

                    if (ZPropertyMesh.IsRuntimable(p))
                        sum =(sum as ICalculable<T>).Add(selector((ZT)(p as IRuntimable).CurValue));
                    else
                        sum = (sum as ICalculable<T>).Add(selector((ZT)p.Value));

                }

                return sum;
            }
            else
            {
                float sum = 0;

                foreach (var p in list)
                {

                    if (ZPropertyMesh.IsRuntimable(p))
                        sum += (float)(object)(selector((ZT)(p as IRuntimable).CurValue));
                    else
                        sum += (float)(object)(selector((ZT)p.Value));

                }
                return (T)(object)sum;
            }

        }


        public IDisposable Sum<ZT>(IZPropertyList list, string multiPropID, Func<ZT, T> selector)
        {

            //check T is valuetype
            if (!typeof(T).IsValueType)
            {
                throw new Exception("Sum is only support value type int and float");
            }

            var plist = ZPropertyMesh.GetPropertiesEx(list, multiPropID);
            this.Value = (T)(object)innerSum(plist, selector);

            listItemValueChanged(plist, _ => this.Value = (T)(object)innerSum(plist, selector));

            //resum the list
            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)innerSum(plist, selector);

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)innerSum(plist, selector);

            var disposable = new ListDisposable(list as IZPropertyList,
                  newItem =>
                  {
                      var v = ZPropertyMesh.GetPropertyEx(newItem, multiPropID);
                      plist.Add(v);
                      this.Value = this.Value = (T)(object)innerSum(plist, selector);
                  },
                 (_, ___) =>
                 {
                     plist = ZPropertyMesh.GetPropertiesEx(list, multiPropID);
                     this.Value = (T)(object)innerSum(plist, selector);
                 }
                 );

            return disposable;
        }


        public IDisposable Sum<ZT>(IZPropertyRefList list, string multiPropID, Func<ZT, T> selector)
        {

            //check T is valuetype
            if (!typeof(T).IsValueType)
            {
                throw new Exception("Sum is only support value type int and float");
            }

            var plist = ZPropertyMesh.GetPropertiesEx(list, multiPropID);
            this.Value = (T)(object)innerSum(plist, selector);

            listItemValueChanged(plist, _ => this.Value = (T)(object)innerSum(plist, selector));

            //resum the list
            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)innerSum(plist, selector);

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)innerSum(plist, selector);

            var disposable = new ListDisposable(list as IZPropertyList,
                  newItem =>
                  {
                      var v = ZPropertyMesh.GetPropertyEx(newItem, multiPropID);
                      plist.Add(v);
                      this.Value = this.Value = (T)(object)innerSum(plist, selector);
                  },
                 (_, ___) =>
                 {
                     plist = ZPropertyMesh.GetPropertiesEx(list, multiPropID);
                     this.Value = (T)(object)innerSum(plist, selector);
                 });

            return disposable;
        }

        //support item selector and value selector
        public IDisposable Sum<IT, ZT>(IZPropertyList<IT> list, string multiPropID, Func<ZT, T> selector, Func<IT, bool> itemSelector)
        {

            //check T is valuetype
            if (!typeof(T).IsValueType)
            {
                throw new Exception("Sum is only support value type int and float");
            }

            var supList = ZPropertyMesh.GetPropertiesEx<IT>(list, multiPropID, itemSelector);
            //var supList = plist.FindAll(a => itemSelector((IT)a.Value));

            this.Value = (T)(object)innerSum(supList, selector);

            listItemValueChanged(supList, _ => this.Value = (T)(object)innerSum(supList, selector));

            //resum the list
            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)innerSum(supList, selector);

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)innerSum(supList, selector);

            var disposable = new ListDisposable(list as IZPropertyList,
                 newItem =>
                 {
                     var v = ZPropertyMesh.GetPropertyEx(newItem, multiPropID);
                     supList.Add(v);
                     this.Value = this.Value = (T)(object)innerSum(supList, selector);
                 },
                 (_, ___) =>
                 {
                     supList = ZPropertyMesh.GetPropertiesEx<IT>(list, multiPropID, itemSelector);
                     this.Value = (T)(object)innerSum(supList, selector);
                 });

            return disposable;
        }

        public IDisposable Sum<IT, ZT>(IZPropertyRefList<IT> list, string multiPropID, Func<ZT, T> selector, Func<IT, bool> itemSelector)
        {

            //check T is valuetype
            if (!typeof(T).IsValueType)
            {
                throw new Exception("Sum is only support value type int and float");
            }

            var supList = ZPropertyMesh.GetPropertiesEx<IT>(list, multiPropID, itemSelector);
            //var supList = plist.FindAll(a => itemSelector((IT)a.Value));

            this.Value = (T)(object)innerSum(supList, selector);

            listItemValueChanged(supList, _ => this.Value = (T)(object)innerSum(supList, selector));

            //resum the list
            //list.OnAddItem += _ => this.Value = this.Value = (T)(object)innerSum(supList, selector);

            //list.OnRemoveItem += (_, ___) => this.Value = (T)(object)innerSum(supList, selector);

            var disposable = new ListDisposable(list as IZPropertyList,
                 newItem =>
                 {
                     var v = ZPropertyMesh.GetPropertyEx(newItem, multiPropID);
                     supList.Add(v);
                     this.Value = this.Value = (T)(object)innerSum(supList, selector);
                 },
                 (_, ___) =>
                 {
                     supList = ZPropertyMesh.GetPropertiesEx<IT>(list, multiPropID, itemSelector);
                     this.Value = (T)(object)innerSum(supList, selector); 
                 });

            return disposable;
        }
    }
}

