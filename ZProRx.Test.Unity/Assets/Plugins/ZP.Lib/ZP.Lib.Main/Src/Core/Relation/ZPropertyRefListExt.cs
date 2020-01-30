using System;
using UnityEngine.Assertions;

namespace ZP.Lib
{
    public partial class ZPropertyRefList<T> : ZProperty<T>, IZPropertyRefList, IZPropertyRefList<T>
    {

        //item is same
        public IDisposable Cache(IZPropertyList<T> source)
        {

            foreach (var v in source.PropList)
            {
                Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                int id = (v.Value as IIndexable).Index;

                Add(id, (T)v.Value);
            }

            //only handle sync add items
            return new OnAddItemDisposable<T>(source, (v) =>
            {
                int id = (v as IIndexable).Index;
                Add(id, (T)v);
            });
        }

        public IDisposable Cache(IZPropertyRefList<T> source)
        {
            foreach (var v in source.PropList)
            {
                ZPropertyInterfaceRef<T> zv = v as ZPropertyInterfaceRef<T>;
                Add(zv);
            }

            //only handle sync add items
            return new OnAddRefItemDisposable<T>(source, (v) =>
            {
                //ZPropertyInterfaceRef<T> zv = v as ZPropertyInterfaceRef<T>;
                int id = (v as IIndexable).Index;
                 Add(id, (T)v);
            });
        }

        public IDisposable Select<ZT>(IZPropertyRefList<ZT> source, Func<ZPropertyInterfaceRef<ZT>, ZPropertyInterfaceRef<T>> selector)
        {

            foreach (var v in source.PropList)
            {
                ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
                Add(selector(zv));
            }

            //source.OnAddItem += (v) =>
            //{
            //    ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
            //    Add(selector(zv));
            //};

            //source.OnRemoveItem += (v, index) =>
            //{
            //    ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
            //    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.RefID);
            //    //Remove(a => a == selector((ZT)v));
            //};

            var disposable = new RefListDisposable(source as IZPropertyRefList,
                 (v) =>
                 {
                     ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
                     Add(selector(zv));
                 },
                 (v, index) =>
                 {
                     ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
                     Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.RefID);
                     //Remove(a => a == selector((ZT)v));
                 });

            return disposable;
        }

        public IDisposable Select<ZT>(IZPropertyRefList<ZT> source, Func<ZPropertyInterfaceRef<ZT>, T> selector)
        {

            foreach (var v in source.PropList)
            {
                ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
                Add(zv.RefID, selector(zv));
            }

            //source.OnAddItem += (v) =>
            //{
            //    ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
            //    Add(zv.RefID, selector(zv));
            //};

            //source.OnRemoveItem += (v, index) =>
            //{
            //    ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
            //    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.RefID);
            //    //Remove(a => a == selector((ZT)v));
            //};

            var disposable = new RefListDisposable(source as IZPropertyRefList,
                (v) =>
                {
                    ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
                    Add(zv.RefID, selector(zv));
                },
                (v, index) =>
                {
                    ZPropertyInterfaceRef<ZT> zv = v as ZPropertyInterfaceRef<ZT>;
                    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.RefID);
                    //Remove(a => a == selector((ZT)v));
                });

            return disposable;
        }

        public IDisposable Select<ZT>(IZPropertyList<ZT> source, Func<ZT, T> selector)
        {
            
            foreach (var v in source.PropList)
            {
                Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                int id = (v.Value as IIndexable).Index;
                if (selector != null)
                    Add(id, selector((ZT)v.Value));
            }

            //source.OnAddItem += (v) =>
            //{
            //    Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
            //    int id = (v.Value as IIndexable).Index;
            //    if (selector == null)
            //        Add(id, selector((ZT)v.Value));
            //};

            //source.OnRemoveItem += (v, index) =>
            //{
            //    int id = (v?.Value as IIndexable).Index;
            //    Remove((ZPropertyInterfaceRef<T> obj) => obj.RefID == id);
            //};

            var disposable = new ListDisposable(source as IZPropertyList,
                (v) =>
                {
                    Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                    int id = (v.Value as IIndexable).Index;
                    if (selector != null)
                        Add(id, selector((ZT)v.Value));
                },
                (v, index) =>
                {
                    int id = (v?.Value as IIndexable).Index;
                    Remove((ZPropertyInterfaceRef<T> obj) => obj.RefID == id);
                });

            return disposable;
        }

        //T must be same source is reflist
        public IDisposable Where(IZPropertyRefList<T> source, Func<ZPropertyInterfaceRef<T>, bool> selector)
        {
            //check type is same
            foreach (var v in source.PropList)
            {
                ZPropertyInterfaceRef<T> zv = v as ZPropertyInterfaceRef<T>;
                if (selector(zv))
                    Add(zv);
            }


            //source.OnAddItem += (v) =>
            //{
            //    ZPropertyInterfaceRef<T> zv = v as ZPropertyInterfaceRef<T>;

            //    if (selector(zv))
            //        Add(zv);
            //};

            //source.OnRemoveItem += (v, index) =>
            //{
            //    ZPropertyInterfaceRef<T> zv = v as ZPropertyInterfaceRef<T>;
            //    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.RefID);
            //    //Remove(a => a == selector((ZT)v));
            //};

            var disposable = new RefListDisposable(source as IZPropertyRefList,
                (v) =>
                {
                    ZPropertyInterfaceRef<T> zv = v as ZPropertyInterfaceRef<T>;

                    if (selector(zv))
                        Add(zv);
                },
                (v, index) =>
                {
                    ZPropertyInterfaceRef<T> zv = v as ZPropertyInterfaceRef<T>;
                    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.RefID);
                    //Remove(a => a == selector((ZT)v));
                });

            return disposable;
        }

        //T must be same
        public IDisposable Where(IZPropertyList<T> source, Func<IZProperty, bool> selector)
        {
            //check type is same


            //if (!typeof(IIndexable).IsAssignableFrom(typeof(T)))
            //{
            //    throw new Exception("List item value must be IIndexable");
            //}

            foreach (var v in source.PropList)
            {
                if (selector(v))
                {
                    Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                    int id = (v.Value as IIndexable).Index;
                    Add(id, (T)v.Value);
                }
            }


            //source.OnAddItem += (IZProperty v) =>
            //{
            //    if (selector(v))
            //    {
            //        Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
            //        int id = (v.Value as IIndexable).Index;
            //        Add(id, (T)v.Value);
            //    }
            //};

            //source.OnRemoveItem += (v, index) =>
            //{
            //    IIndexable zv = v.Value as IIndexable;
            //    Assert.IsNotNull(zv, "It 's not a Indexable " + this.PropertyID);

            //    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.Index);
            //    //Remove(a => a == selector((ZT)v));
            //};

            var disposable = new ListDisposable(source as IZPropertyList,
                (IZProperty v) =>
                {
                    if (selector(v))
                    {
                        Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                        int id = (v.Value as IIndexable).Index;
                        Add(id, (T)v.Value);
                    }
                }, (v, index) =>
                {
                    IIndexable zv = v.Value as IIndexable;
                    Assert.IsNotNull(zv, "It 's not a Indexable " + this.PropertyID);

                    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.Index);
                    //Remove(a => a == selector((ZT)v));
                });

            return disposable;

        }

        public IDisposable Sort(IZPropertyRefList<T> source, Comparison<T> comparer)
        {

            foreach (var v in source.PropList)
            {
                //Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                int? id = (v.Value as IRefable)?.RefID;
                if (id != null)
                    Add((int)id, (T)v.Value);
            }

            PropList.Sort((p, p1) => comparer((T)p.Value, (T)p1.Value));


            //source.OnAddItem += (IZProperty v) =>
            //{
            //    int? id = (v.Value as IRefable)?.RefID;
            //    if (id != null)
            //        Add((int)id, (T)v.Value);

            //    //resort
            //    PropList.Sort((p, p1) => comparer((T)p.Value, (T)p1.Value));
            //};

            //source.OnRemoveItem += (v, index) =>
            //{
            //    int? id = (v.Value as IRefable)?.RefID;

            //    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == id);
            //    //Remove(a => a == selector((ZT)v));
            //};

            var disposable = new RefListDisposable(source as IZPropertyRefList,
                   (IZProperty v) =>
                   {
                       int? id = (v.Value as IRefable)?.RefID;
                       if (id != null)
                           Add((int)id, (T)v.Value);

                       //resort
                       PropList.Sort((p, p1) => comparer((T)p.Value, (T)p1.Value));
                   },
                    (v, index) =>
                    {
                        int? id = (v.Value as IRefable)?.RefID;

                        Remove((ZPropertyInterfaceRef<T> a) => a.RefID == id);
                        //Remove(a => a == selector((ZT)v));
                    });
            return disposable;


        }


        public IDisposable Sort(IZPropertyList<T> source, Comparison<T> comparer)
        {

            foreach (var v in source.PropList)
            {
                Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                int id = (v.Value as IIndexable).Index;
                Add(id, (T)v.Value);
            }

            PropList.Sort((p, p1) => comparer((T)p.Value, (T)p1.Value));


            //source.OnAddItem += (IZProperty v) =>
            //{
            //    Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
            //    int id = (v.Value as IIndexable).Index;
            //    Add(id, (T)v.Value);

            //    //resort
            //    PropList.Sort((p, p1) => comparer((T)p.Value, (T)p1.Value));
            //};

            //source.OnRemoveItem += (v, index) =>
            //{
            //    IIndexable zv = v.Value as IIndexable;
            //    Assert.IsNotNull(zv, "It 's not a Indexable " + this.PropertyID);

            //    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.Index);
            //    //Remove(a => a == selector((ZT)v));
            //};

            var disposable = new ListDisposable(source as IZPropertyList,
                (IZProperty v) =>
                {
                    Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                    int id = (v.Value as IIndexable).Index;
                    Add(id, (T)v.Value);

                    //resort
                    PropList.Sort((p, p1) => comparer((T)p.Value, (T)p1.Value));
                },
                (v, index) =>
                {
                    IIndexable zv = v.Value as IIndexable;
                    Assert.IsNotNull(zv, "It 's not a Indexable " + this.PropertyID);

                    Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.Index);
                    //Remove(a => a == selector((ZT)v));
                });
            return disposable;
        }



        public IDisposable Merge<T1>(IZPropertyList<T1> source1, IZPropertyList<T1> source2, Func<T1, bool> selector = null) where T1 : T, IIndexable
        {

            foreach (var v in source1.PropList)
            {
                Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                int id = (v.Value as IIndexable).Index;
                if (selector == null || selector((T1)v.Value))
                    Add(id, (T)v.Value);
            }

            foreach (var v in source2.PropList)
            {
                Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                int id = (v.Value as IIndexable).Index;
                if (selector == null || selector((T1)v.Value))
                    Add(id, (T)v.Value);
                //ZPropertyInterfaceRef<T> zv = v as ZPropertyInterfaceRef<T>;

                //Assert.IsNotNull(zv, "It 's not a ZPropertyInterfaceRef " + this.PropertyID);

                ////if (selector(zv))
                //if (selector == null || selector((T1)zv.Value))
                //Add(zv);
            }

            MultiDisposable disposables = new MultiDisposable();
            disposables.Add(new ListDisposable(source1 as IZPropertyList, (IZProperty v) =>
            {
                //if (selector(v))
                {
                    Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                    int id = (v.Value as IIndexable).Index;
                    // Assert.IsNotNull(id, "It 's not a ZPropertyInterfaceRef " + this.PropertyID);
                    if (selector == null || selector((T1)v.Value))
                        Add(id, (T)v.Value);
                }
            }, (v, index) =>
            {
                Assert.IsNotNull((v as IIndexable), "It 's not a Indexable " + this.PropertyID);
                IIndexable zv = v.Value as IIndexable;
                Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.Index);
                //Remove(a => a == selector((ZT)v));
            }));

            disposables.Add(new ListDisposable(source2 as IZPropertyList, (IZProperty v) =>
            {
                //if (selector(v))
                {
                    Assert.IsNotNull((v.Value as IIndexable), "It 's not a Indexable " + this.PropertyID);
                    int id = (v.Value as IIndexable).Index;
                    if (selector == null || selector((T1)v.Value))
                        Add(id, (T)v.Value);
                }
            },(v, index) =>
            {
                Assert.IsNotNull((v as IIndexable), "It 's not a Indexable " + this.PropertyID);
                IIndexable zv = v.Value as IIndexable;
                Remove((ZPropertyInterfaceRef<T> a) => a.RefID == zv.Index);
                //Remove(a => a == selector((ZT)v));
            }));

            return disposables;
        }
    }
}

