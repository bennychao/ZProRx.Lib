using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    public interface IZPropertyRefList : IEnumerable
    {
        int Count { get; }

        void Add(int refid);
        void RemoveAt(int index);
        void Add(IIndexable indexer);
        void RemoveObject(Predicate<object> comparer);

        List<IZProperty> PropList { get; }

        object ConvertToArray();
        void BindRefs();

        void ClearAll();

        //Array
        Action<IZProperty> OnAddItem { set; get; }
        Action<IZProperty, int> OnRemoveItem { set; get; }
    }

    public interface IZPropertyRefList<T> : IEnumerable<T>
    {
        int Count { get; }


        void Add(int refid);
        void RemoveAt(int index);
        void Remove(Predicate<T> comparer);
        void Remove(Predicate<ZPropertyInterfaceRef<T>> comparer);

        List<IZProperty> PropList { get; }

        IList<T> ConvertToArray();
        void BindRefs();
        void ClearAll();

        //Array
        Action<IZProperty> OnAddItem { set; get; }
        Action<IZProperty, int> OnRemoveItem { set; get; }
    }
}
