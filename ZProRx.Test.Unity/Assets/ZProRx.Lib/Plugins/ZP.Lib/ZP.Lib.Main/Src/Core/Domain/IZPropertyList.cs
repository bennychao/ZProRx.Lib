using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    public interface IZPropertyList : IEnumerable
    {

        int Count { get; }

        void Add(object propData);
        void RemoveAt(int index);

        void RemoveObject(Predicate<object> comparer);

        List<IZProperty> PropList { get; }

        void ClearAll();
        object ConvertToArray();

        //Array
        Action<IZProperty> OnAddItem { set; get; }
        Action<IZProperty, int> OnRemoveItem { set; get; }

    }

    public interface IZPropertyList<T> : IEnumerable<T>
    {

        int Count { get; }

        void Add(T propData);
        void RemoveAt(int index);
        void Remove(Predicate<T> comparer);

        List<IZProperty> PropList { get; }

        void ClearAll();
        IEnumerable<T> ConvertToArray();


        Action<IZProperty> OnAddItem { set; get; }
        Action<IZProperty, int> OnRemoveItem { set; get; }
    }
}
