using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Operators;
using System;
using UnityEngine.Events;
using System.Linq;
using ZP.Lib.Core.Domain;

#if ZP_UNIRX
using UniRx;
#endif

namespace ZP.Lib
{
#if ZP_UNIRX


    public static class ZPropertyObservable
    {
        static public IObservable<T> ValueChange<T>(IZProperty<T> prop)
        {
            //Subject<T> subject = new Subject<T>();

            return Observable.FromEvent<T>(h => prop.OnValueChanged += h, h => prop.OnValueChanged -= h);
        }

        static public IObservable<T> ValueChangeAsObservable<T>(this IZProperty<T> prop)
        {
            //Subject<T> subject = new Subject<T>();

            return Observable.FromEvent<T>(h => prop.OnValueChanged += h, h => prop.OnValueChanged -= h);
        }


        static public IObservable<object> OnValueChange(IZProperty prop)
        {
            return Observable.FromEvent<object>(h => prop.OnValueChanged += h, h => prop.OnValueChanged -= h);
        }

        static public IObservable<object> ValueChangeAsObservable(this IZProperty prop)
        {
            return Observable.FromEvent<object>(h => prop.OnValueChanged += h, h => prop.OnValueChanged -= h);
        }


        static public IObservable<IZEvent> SubEvents(object obj, string multiEventId)
        {
            var eventList = ZPropertyMesh.GetEventsEx(obj, multiEventId); //TODO
            return Observable.Merge(eventList.ToList().Select(e => e.OnEventObservable().Select(_=> e)));
        }

        //ListSubEventAsObservable only support list item's Event
        static public IObservable<T> SubEvents<T>(object obj, string multiEventId)
        {
            var eventList = ZPropertyMesh.GetEventsEx(obj, multiEventId); //TODO
            return Observable.Merge(eventList.ToList().Select(e => (e as IZEvent<T>)?.OnEventObservable()).Where(e => e != null));
        }

        static public IObservable<IZEvent> SubEventsAsObservable(this IZProperty prop, string multiEventId)
        {
            var eventList = ZPropertyMesh.GetEventsEx(prop.Value, multiEventId); //TODO
            return Observable.Merge(eventList.ToList().Select(e => e.OnEventObservable().Select(_ => e)));
        }

        static public IObservable<T> SubEventsAsObservable<T>(this IZProperty prop, string multiEventId)
        {
            var eventList = ZPropertyMesh.GetEventsEx(prop.Value, multiEventId); //TODO
            return Observable.Merge(eventList.ToList().Select(e => (e as IZEvent<T>)?.OnEventObservable()).Where(e => e != null));
        }

        static public IObservable<T>  SelectParent<T>(this IObservable<IZEvent> ev) where T : class
        {
            return ev.Select(e => (e as IDirectLinkable).Parent as T);
        }

        //static public IObservable<T> ListValueChange<T>(IZPropertyList<T> prop)
        //{
        //    CompositeDisposable disposables = new CompositeDisposable();
        //    return
        //}

        static public IObservable<Unit> OnAction(Action action)
        {
            return Observable.FromEvent(h => action += h, h => action -= h);
        }

        static public IObservable<Unit> OnEventObservable(this IZEvent zEvent)
        {
            return Observable.FromEvent(h => zEvent.OnEvent += h, h => zEvent.OnEvent -= h);
        }

        static public IObservable<T> OnEventObservable<T>(this IZEvent<T> zEvent)
        {
            return Observable.FromEvent<Action<T>, T>(h => a => h(a), h => zEvent.OnEvent += h, h => zEvent.OnEvent -= h);
        }

        static public IObservable<Unit> OnUnityEvent(UnityEvent uEvent)
        {
            return Observable.FromEvent<UnityAction>(h => ()=>h(), h => uEvent.AddListener(h), h => uEvent.RemoveListener(h));
        }

        //T is value not property
        static public IObservable<T> OnAddItem<T>(IZPropertyList<T> prop)
        {
            return new ListAddItemObservable<T>(prop);
        }

        static public IObservable<T> AddItemAsObservable<T>(this IZPropertyList<T> prop)
        {
            return new ListAddItemObservable<T>(prop);
        }

        static public IObservable<IZProperty> AddItemAsObservable(this IZPropertyList prop)
        {
            return Observable.FromEvent<Action<IZProperty>, IZProperty>(h => p => h(p), h => prop.OnAddItem += h, h => prop.OnAddItem -= h);
        }

        static public IObservable<T> AddItemAsObservable<T>(this IZPropertyRefList<T> prop)
        {
            return new ListAddRefItemObservable<T>(prop);
        }

        static public IObservable<IZProperty> AddItemAsObservable(this IZPropertyRefList prop)
        {
            return Observable.FromEvent<Action<IZProperty>, IZProperty>(h => p => h(p), h => prop.OnAddItem += h, h => prop.OnAddItem -= h);
        }


        //unirx not support 2 params for action, so not support the delete item's index return
        //T is value
        static public IObservable<T> OnDeleteItem<T>(IZPropertyList<T> prop)
        {
            return new ListDeleteItemObservable<T>(prop);
        }

        static public IObservable<T> DeleteItemAsObservable<T>(this IZPropertyList<T> prop)
        {
            return new ListDeleteItemObservable<T>(prop);
        }

        //static public IObservable<IZProperty> DeleteItemAsObservable(this IZPropertyList prop)
        //{
        //    return Observable.FromEvent<Action<IZProperty, int>, IZProperty, int>(h => p => h(p), h => prop.OnRemoveItem += h, h => prop.OnRemoveItem -= h);
        //}

        static public IObservable<T> DeleteItemAsObservable<T>(this IZPropertyRefList<T> prop)
        {
            return new ListDeleteRefItemObservable<T>(prop);
        }

        static public IObservable<T> ListSubEvent<T>(IZPropertyList<T> prop, string propID)
        {
            return new ListSubEventObservable<T>(prop, propID);
        }

        static public IObservable<T> ListSubEventAsObservable<T>(this IZPropertyList<T> prop, string propID)
        {
            return new ListSubEventObservable<T>(prop, propID);
        }

        static public IObservable<T> RefListSubEvent<T>(IZPropertyRefList<T> prop, string propID)
        {
            return new RefListSubEventObservable<T>(prop, propID);
        }

        static public IObservable<T> RefListSubEventAsObservable<T>(this IZPropertyRefList<T> prop, string propID)
        {
            return new RefListSubEventObservable<T>(prop, propID);
        }

        static public IObservable<T> ListValueChange<T>(IZPropertyRefList<T> prop, string propID)
        {
            return new RefListValueChangeObservable<T>(prop, propID);
        }

        static public IObservable<T> ListValueChange<T>(IZPropertyList<T> prop, string propID)
        {
            return new ListValueChangeObservable<T>(prop, propID);
        }

        static public IObservable<T> ListValueChangeAsObservable<T>(this IZPropertyRefList<T> prop, string propID)
        {
            return new RefListValueChangeObservable<T>(prop, propID);
        }

        static public IObservable<T> ListValueChangeAsObservable<T>(this IZPropertyList<T> prop, string propID)
        {
            return new ListValueChangeObservable<T>(prop, propID);
        }

        static public IObservable<ZTransform> TransformChange(ZTransform trans)
        {
            //Subject<ZTransform> subject = new Subject<ZTransform>();
            return ValueChange<Vector2>(trans.Position).Merge(ValueChange<Vector2>(trans.Direction)).Select(_ => trans);
            //return subject;
        }

        static public IObservable<ZTransform> TransformChangeAsObservable(this ZTransform trans)
        {
            //Subject<ZTransform> subject = new Subject<ZTransform>();
            return ValueChange<Vector2>(trans.Position).Merge(ValueChange<Vector2>(trans.Direction)).Select(_ => trans);
            //return subject;
        }
    }



    public class ListAddItemObservable<T> : OperatorObservableBase<T>
    {
        IZPropertyList<T> prop;
        public ListAddItemObservable(IZPropertyList<T> prop) : base(false)
        {
            this.prop = prop;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Inner(this, observer);
        }

        class Inner: IDisposable
        {
            ListAddItemObservable<T> parent;
            readonly IObserver<T> observer;

            public Inner(ListAddItemObservable<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;
                parent.prop.OnAddItem += OnNext;
            }

            public void Dispose()
            {
                parent.prop.OnAddItem -= OnNext;
            }

            void OnNext(IZProperty p)
            {
                observer.OnNext((T)p.Value);
                //observer.OnCompleted();
            }
        }
    }


    public class ListDeleteItemObservable<T> : OperatorObservableBase<T>
    {
        IZPropertyList<T> prop;
        public ListDeleteItemObservable(IZPropertyList<T> prop) : base(false)
        {
            this.prop = prop;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Inner(this, observer);
        }

        class Inner : IDisposable
        {
            ListDeleteItemObservable<T> parent;
            readonly IObserver<T> observer;

            public Inner(ListDeleteItemObservable<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;
                parent.prop.OnRemoveItem += OnNext;
            }

            public void Dispose()
            {
                parent.prop.OnRemoveItem -= OnNext;
            }

            void OnNext(IZProperty p, int index)
            {
                observer.OnNext((T)p.Value);
                //observer.OnCompleted();
            }
        }
    }


    public class ListAddRefItemObservable<T> : OperatorObservableBase<T>
    {
        IZPropertyRefList<T> prop;
        public ListAddRefItemObservable(IZPropertyRefList<T> prop) : base(false)
        {
            this.prop = prop;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Inner(this, observer);
        }

        class Inner : IDisposable
        {
            ListAddRefItemObservable<T> parent;
            readonly IObserver<T> observer;

            public Inner(ListAddRefItemObservable<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;
                parent.prop.OnAddItem += OnNext;
            }

            public void Dispose()
            {
                parent.prop.OnAddItem -= OnNext;
            }

            void OnNext(IZProperty p)
            {
                observer.OnNext((T)p.Value);
                //observer.OnCompleted();
            }
        }
    }


    public class ListDeleteRefItemObservable<T> : OperatorObservableBase<T>
    {
        IZPropertyRefList<T> prop;
        public ListDeleteRefItemObservable(IZPropertyRefList<T> prop) : base(false)
        {
            this.prop = prop;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Inner(this, observer);
        }

        class Inner : IDisposable
        {
            ListDeleteRefItemObservable<T> parent;
            readonly IObserver<T> observer;

            public Inner(ListDeleteRefItemObservable<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
                this.observer = observer;
                parent.prop.OnRemoveItem += OnNext;
            }

            public void Dispose()
            {
                parent.prop.OnRemoveItem -= OnNext;
            }

            void OnNext(IZProperty p, int index)
            {
                observer.OnNext((T)p.Value);
                //observer.OnCompleted();
            }
        }
    }


    public class ListSubEventObservable<T> : OperatorObservableBase<T>
    {
        IZPropertyList<T> prop;
        readonly string propID;
        public ListSubEventObservable(IZPropertyList<T> prop,  string propID) : base(false)
        {
            this.prop = prop;
            this.propID = propID;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ListDisposable(this, prop as IZPropertyList<T>, observer, propID);
        }

        public class ListDisposable : IDisposable
        {
            List<Inner> disposeList = new List<Inner>();
            private ListSubEventObservable<T> parent;
            private IZPropertyList<T> prop;

            readonly IObserver<T> observer;

            readonly string propID;

            public ListDisposable(ListSubEventObservable<T> parent, 
            IZPropertyList<T> prop, IObserver<T> observer, string propID)
            {
                this.prop = prop;
                this.parent = parent;
                this.observer = observer;
                this.propID = propID;

                foreach (var obj in prop)
                {
                    var e = ZPropertyMesh.GetEventEx(obj, propID);
                    var inner = new Inner((T)obj, e, this.observer);

                    disposeList.Add(inner);
                }

                prop.OnAddItem += onAddHandler;
                prop.OnRemoveItem += onDeleteHandler;
            }

            void onAddHandler(IZProperty obj)
            {
                if (obj != null && obj.Value != null)
                    return;

                var e = ZPropertyMesh.GetEventEx(obj.Value, propID);
                var inner = new Inner((T)obj.Value, e, this.observer);

                disposeList.Add(inner);
            }

            void onDeleteHandler(IZProperty obj, int index)
            {
                if (index >= 0)
                {
                    disposeList[index].Dispose();
                    disposeList.RemoveAt(index);
                }

            }

            public void Dispose()
            {
                prop.OnAddItem -= onAddHandler;

                prop.OnRemoveItem -= onDeleteHandler;

                foreach (var p in disposeList)
                {
                    p.Dispose();
                }
            }
        }

        class Inner : IDisposable
        {
            public T value;
            public IZEvent zEvent;
            readonly IObserver<T> observer;

            public Inner(T value, IZEvent zEvent, IObserver<T> observer)
            {
                this.value = value;
                this.zEvent = zEvent;
                this.observer = observer;

                zEvent.OnEvent += OnNext;
            }

            public void Dispose()
            {
                zEvent.OnEvent -= OnNext;
            }

            void OnNext()
            {
                observer.OnNext(value);
                //observer.OnCompleted();
            }
        }
    }



    public class RefListSubEventObservable<T> : OperatorObservableBase<T>
    {
        IZPropertyRefList<T> prop;
        readonly string propID;
        public RefListSubEventObservable(IZPropertyRefList<T> prop, string propID) : base(false)
        {
            this.prop = prop;
            this.propID = propID;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ListDisposable(this, prop as IZPropertyRefList<T>, observer, propID);
        }

        public class ListDisposable : IDisposable
        {
            List<Inner> disposeList = new List<Inner>();
            private RefListSubEventObservable<T> parent;
            private IZPropertyRefList<T> prop;

            readonly IObserver<T> observer;

            readonly string propID;

            public ListDisposable(RefListSubEventObservable<T> parent,
            IZPropertyRefList<T> prop, IObserver<T> observer, string propID)
            {
                this.prop = prop;
                this.parent = parent;
                this.observer = observer;
                this.propID = propID;

                prop.OnAddItem += onAddHandler;
                prop.OnRemoveItem += onDeleteHandler;

                foreach (var p in prop)
                {
                    var e = ZPropertyMesh.GetEventEx(p, propID);
                    var inner = new Inner(p, e, this.observer);

                    disposeList.Add(inner);
                }
            }

            void onAddHandler(IZProperty obj)
            {
                if (obj != null && obj.Value != null)
                    return;

                var e = ZPropertyMesh.GetEventEx(obj.Value, propID);
                var inner = new Inner((T)obj.Value, e, this.observer);

                disposeList.Add(inner);
            }

            void onDeleteHandler(IZProperty obj, int index)
            {
                if (index >= 0)
                {
                    disposeList[index].Dispose();
                    disposeList.RemoveAt(index);
                }

            }

            public void Dispose()
            {
                prop.OnAddItem -= onAddHandler;

                prop.OnRemoveItem -= onDeleteHandler;

                foreach (var p in disposeList)
                {
                    p.Dispose();
                }
            }
        }

        class Inner : IDisposable
        {
            public T value;
            public IZEvent zEvent;
            readonly IObserver<T> observer;

            public Inner(T value, IZEvent zEvent, IObserver<T> observer)
            {
                this.value = value;
                this.zEvent = zEvent;
                this.observer = observer;

                zEvent.OnEvent += OnNext;
            }

            public void Dispose()
            {
                zEvent.OnEvent -= OnNext;
            }

            void OnNext()
            {
                observer.OnNext(value);
                //observer.OnCompleted();
            }
        }
    }



    public class RefListValueChangeObservable<T> : OperatorObservableBase<T>
    {
        IZPropertyRefList<T> prop;
        readonly string propID;
        public RefListValueChangeObservable(IZPropertyRefList<T> prop, string propID) : base(false)
        {
            this.prop = prop;
            this.propID = propID;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ListDisposable(this, prop as IZPropertyRefList<T>, observer, propID);
        }

        public class ListDisposable : IDisposable
        {
            List<Inner> disposeList = new List<Inner>();
            private RefListValueChangeObservable<T> parent;
            private IZPropertyRefList<T> prop;

            readonly IObserver<T> observer;

            readonly string propID;

            public ListDisposable(RefListValueChangeObservable<T> parent,
            IZPropertyRefList<T> prop, IObserver<T> observer, string propID)
            {
                this.prop = prop;
                this.parent = parent;
                this.observer = observer;
                this.propID = propID;

                prop.OnAddItem += onAddHandler;
                prop.OnRemoveItem += onDeleteHandler;

                foreach (var p in prop)
                {
                    var e = ZPropertyMesh.GetPropertyEx(p, propID);
                    var inner = new Inner(p, e, this.observer);

                    disposeList.Add(inner);
                }
            }

            void onAddHandler(IZProperty obj)
            {
                if (obj != null && obj.Value != null)
                    return;

                var e = ZPropertyMesh.GetPropertyEx(obj.Value, propID);
                var inner = new Inner((T)obj.Value, e, this.observer);

                disposeList.Add(inner);
            }

            void onDeleteHandler(IZProperty obj, int index)
            {
                if (index >= 0)
                {
                    disposeList[index].Dispose();
                    disposeList.RemoveAt(index);
                }

            }

            public void Dispose()
            {
                prop.OnAddItem -= onAddHandler;

                prop.OnRemoveItem -= onDeleteHandler;

                foreach (var p in disposeList)
                {
                    p.Dispose();
                }
            }
        }

        class Inner : IDisposable
        {
            public T value;
            public IZProperty prop;
            readonly IObserver<T> observer;

            public Inner(T value, IZProperty prop, IObserver<T> observer)
            {
                this.value = value;
                this.prop = prop;
                this.observer = observer;

                prop.OnValueChanged += OnValueChange;
            }

            public void Dispose()
            {
                prop.OnValueChanged -= OnValueChange;
            }

            void OnValueChange(object obj)
            {
                observer.OnNext(value);
                //observer.OnCompleted();
            }
        }
    }


    public class ListValueChangeObservable<T> : OperatorObservableBase<T>
    {
        IZPropertyList<T> prop;
        readonly string propID;
        public ListValueChangeObservable(IZPropertyList<T> prop, string propID) : base(false)
        {
            this.prop = prop;
            this.propID = propID;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ListDisposable(this, prop as IZPropertyList<T>, observer, propID);
        }

        public class ListDisposable : IDisposable
        {
            List<Inner> disposeList = new List<Inner>();
            private ListValueChangeObservable<T> parent;
            private IZPropertyList<T> prop;

            readonly IObserver<T> observer;

            readonly string propID;

            public ListDisposable(ListValueChangeObservable<T> parent,
            IZPropertyList<T> prop, IObserver<T> observer, string propID)
            {
                this.prop = prop;
                this.parent = parent;
                this.observer = observer;
                this.propID = propID;

                prop.OnAddItem += onAddHandler;
                prop.OnRemoveItem += onDeleteHandler;

                foreach (var p in prop)
                {
                    var e = ZPropertyMesh.GetPropertyEx(p, propID);
                    var inner = new Inner(p, e, this.observer);

                    disposeList.Add(inner);
                }
            }

            void onAddHandler(IZProperty obj)
            {
                if (obj != null && obj.Value != null)
                    return;

                var e = ZPropertyMesh.GetPropertyEx(obj.Value, propID);
                var inner = new Inner((T)obj.Value, e, this.observer);

                disposeList.Add(inner);
            }

            void onDeleteHandler(IZProperty obj, int index)
            {
                if (index >= 0)
                {
                    disposeList[index].Dispose();
                    disposeList.RemoveAt(index);
                }

            }

            public void Dispose()
            {
                prop.OnAddItem -= onAddHandler;

                prop.OnRemoveItem -= onDeleteHandler;

                foreach (var p in disposeList)
                {
                    p.Dispose();
                }
            }
        }

        class Inner : IDisposable
        {
            public T value;
            public IZProperty prop;
            readonly IObserver<T> observer;

            public Inner(T value, IZProperty prop, IObserver<T> observer)
            {
                this.value = value;
                this.prop = prop;
                this.observer = observer;

                prop.OnValueChanged += OnValueChange;
            }

            public void Dispose()
            {
                prop.OnValueChanged -= OnValueChange;
            }

            void OnValueChange(object obj)
            {
                observer.OnNext(value);
                //observer.OnCompleted();
            }
        }
    }

#endif
}