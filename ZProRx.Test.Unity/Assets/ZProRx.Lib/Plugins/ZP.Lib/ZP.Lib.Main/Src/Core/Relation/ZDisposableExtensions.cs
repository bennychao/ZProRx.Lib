using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{

    public static class ZDisposableExtensions
    {
        public static T AddToMultiDisposable<T>(this T disposable, ICollection<IDisposable> container)
            where T : IDisposable
        {
            if (disposable == null) throw new ArgumentNullException("disposable");
            if (container == null) throw new ArgumentNullException("container");

            container.Add(disposable);

            return disposable;
        }
    }

    public class MultiDisposable : IDisposable, ICollection<IDisposable>
    {
        List<IDisposable> plist = new List<IDisposable>();

        public int Count
        {
            get;set;
        }

        public bool IsReadOnly
        {
            get;set;
        }

        public void Add(IDisposable item)
        {
            plist.Add(item);
        }



        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IDisposable item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (var p in plist)
            {
                p.Dispose();
            }
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(IDisposable item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }


    //Action<object>
    public class OnValueChangedDisposable : IDisposable
    {
        private IZProperty prop;
        private Action<object> onHandle;
        public OnValueChangedDisposable(IZProperty prop, Action<object> onHandle) {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnValueChanged += OnHander;
        }

        void OnHander(object obj) {
            if (onHandle != null)
                onHandle(obj);
        }

        public void Dispose()
        {
            prop.OnValueChanged -= OnHander;
        }

    }

    //Action<T>
    //FromEvent<T>
    public class OnValueChangedDisposable<T> : IDisposable
    {
        private IZProperty<T> prop;
        private Action<T> onHandle;
        public OnValueChangedDisposable(IZProperty<T> prop, Action<T> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnValueChanged += OnHander;
        }

        void OnHander(T obj)
        {
            if (onHandle != null)
                onHandle(obj);
        }

        public void Dispose()
        {
            prop.OnValueChanged -= OnHander;
        }

    }

    //list 
    public class ListDisposable : IDisposable
    {
        private IZPropertyList prop;
        private Action<IZProperty> onAddHandle;
        private Action<IZProperty, int> onDeleteHandle;
        public ListDisposable(IZPropertyList prop, Action<IZProperty> onAddHandle, Action<IZProperty, int> onDeleteHandle)
        {
            this.prop = prop;
            this.onAddHandle = onAddHandle;
            this.onDeleteHandle = onDeleteHandle;

            prop.OnAddItem += onAddHandler;
            prop.OnRemoveItem += onDeleteHandler;
        }

        void onAddHandler(IZProperty obj)
        {
            if (onAddHandle != null)  //&& obj != null   /// list 'value  can be null!!!
                onAddHandle(obj);
        }

        void onDeleteHandler(IZProperty obj, int index)
        {
            if (onDeleteHandle != null)
                onDeleteHandle(obj, index);
        }

        public void Dispose()
        {
            prop.OnAddItem -= onAddHandler;

            prop.OnRemoveItem -= onDeleteHandler;
        }
    }


    public class RefListDisposable : IDisposable
    {
        private IZPropertyRefList prop;
        private Action<IZProperty> onAddHandle;
        private Action<IZProperty, int> onDeleteHandle;
        public RefListDisposable(IZPropertyRefList prop, Action<IZProperty> onAddHandle, Action<IZProperty, int> onDeleteHandle)
        {
            this.prop = prop;
            this.onAddHandle = onAddHandle;
            this.onDeleteHandle = onDeleteHandle;

            prop.OnAddItem += onAddHandler;
            prop.OnRemoveItem += onDeleteHandler;
        }

        void onAddHandler(IZProperty obj)
        {
            if (onAddHandle != null)  //&& obj != null   /// list 'value  can be null!!!
                onAddHandle(obj);
        }

        void onDeleteHandler(IZProperty obj, int index)
        {
            if (onDeleteHandle != null)
                onDeleteHandle(obj, index);
        }

        public void Dispose()
        {
            prop.OnAddItem -= onAddHandler;

            prop.OnRemoveItem -= onDeleteHandler;
        }
    }

    //Action<IZProperty>
    public class OnAddItemDisposable: IDisposable
    {
        private IZPropertyList prop;
        private Action<IZProperty> onHandle;
        public OnAddItemDisposable(IZPropertyList prop, Action<IZProperty> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnAddItem += OnHander;
        }

        void OnHander(IZProperty obj)
        {
            if (onHandle != null)  //&& obj != null   /// list 'value  can be null!!!
                onHandle(obj);
        }

        public void Dispose()
        {
            prop.OnAddItem -= OnHander;
        }
    }

    //Action<T>
    public class OnAddItemDisposable<T> : IDisposable
    {
        private IZPropertyList<T> prop;
        private Action<T> onHandle;
        public OnAddItemDisposable(IZPropertyList<T> prop, Action<T> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnAddItem += OnHander;
        }

        void OnHander(IZProperty obj)
        {
            if (onHandle != null && obj != null)
                onHandle((T)obj.Value);
        }

        public void Dispose()
        {
            prop.OnAddItem -= OnHander;
        }
    }

    //Action<IZProperty, int>
    public class OnDeleteItemDisposable : IDisposable
    {
        private IZPropertyList prop;
        private Action<IZProperty, int> onHandle;
        public OnDeleteItemDisposable(IZPropertyList prop, Action<IZProperty, int> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnRemoveItem += OnHander;
        }

        void OnHander(IZProperty obj, int index)
        {
            if (onHandle != null)  //&& obj != null   /// list 'value  can be null!!!
                onHandle(obj, index);
        }

        public void Dispose()
        {
            prop.OnRemoveItem -= OnHander;
        }
    }

    //Action<T, int>
    public class OnDeleteItemDisposable<T> : IDisposable
    {
        private IZPropertyList<T> prop;
        private Action<T, int> onHandle;
        public OnDeleteItemDisposable(IZPropertyList<T> prop, Action<T, int> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnRemoveItem += OnHander;
        }

        void OnHander(IZProperty obj, int index)
        {
            if (onHandle != null)  //&& obj != null   /// list 'value  can be null!!!
                onHandle((T)obj.Value, index);
        }

        public void Dispose()
        {
            prop.OnRemoveItem -= OnHander;
        }
    }




    //Action<IZProperty>
    public class OnAddRefItemDisposable : IDisposable
    {
        private IZPropertyRefList prop;
        private Action<IZProperty> onHandle;
        public OnAddRefItemDisposable(IZPropertyRefList prop, Action<IZProperty> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnAddItem += OnHander;
        }

        void OnHander(IZProperty obj)
        {
            if (onHandle != null)  //&& obj != null   /// list 'value  can be null!!!
                onHandle(obj);
        }

        public void Dispose()
        {
            prop.OnAddItem -= OnHander;
        }
    }

    //Action<T>
    public class OnAddRefItemDisposable<T> : IDisposable
    {
        private IZPropertyRefList<T> prop;
        private Action<T> onHandle;
        public OnAddRefItemDisposable(IZPropertyRefList<T> prop, Action<T> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnAddItem += OnHander;
        }

        void OnHander(IZProperty obj)
        {
            if (onHandle != null && obj != null)
                onHandle((T)obj.Value);
        }

        public void Dispose()
        {
            prop.OnAddItem -= OnHander;
        }
    }

    //Action<IZProperty, int>
    public class OnDeleteRefItemDisposable : IDisposable
    {
        private IZPropertyRefList prop;
        private Action<IZProperty, int> onHandle;
        public OnDeleteRefItemDisposable(IZPropertyRefList prop, Action<IZProperty, int> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnRemoveItem += OnHander;
        }

        void OnHander(IZProperty obj, int index)
        {
            if (onHandle != null)  //&& obj != null   /// list 'value  can be null!!!
                onHandle(obj, index);
        }

        public void Dispose()
        {
            prop.OnRemoveItem -= OnHander;
        }
    }

    //Action<T, int>
    public class OnDeleteRefItemDisposable<T> : IDisposable
    {
        private IZPropertyRefList<T> prop;
        private Action<T, int> onHandle;
        public OnDeleteRefItemDisposable(IZPropertyRefList<T> prop, Action<T, int> onHandle)
        {
            this.prop = prop;
            this.onHandle = onHandle;
            prop.OnRemoveItem += OnHander;
        }

        void OnHander(IZProperty obj, int index)
        {
            if (onHandle != null)  //&& obj != null   /// list 'value  can be null!!!
                onHandle((T)obj.Value, index);
        }

        public void Dispose()
        {
            prop.OnRemoveItem -= OnHander;
        }
    }
}
