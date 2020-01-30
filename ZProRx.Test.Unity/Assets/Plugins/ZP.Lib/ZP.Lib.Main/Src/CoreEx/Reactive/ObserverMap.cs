using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using ZP.Lib.CoreEx.Domain;

namespace ZP.Lib.CoreEx.Reactive
{
    public class ObserverMap<TValue> : List<KeyValuePair<string, ICancellableObserver<TValue>  >>
    {
        protected readonly Func<string, ICancellableObserver<TValue>> CreateFunc = null;

        public ObserverMap(Func<string, ICancellableObserver<TValue>> createFunc = null)
        {
            this.CreateFunc = createFunc;
        }

        public ICancellableObserver<TValue> CreateObserver(string key)
        {
            ICancellableObserver<TValue> ret = null;

            //default create function
            if (this.CreateFunc == null)
            {
                ret = new Subject<TValue>().ToCancellable<TValue>();

                //auto remove from map
                ret.Token.Register(() =>
                {
                    RemoveObserver(key);
                });
            }


            ret = CreateFunc(key);

            Add(new KeyValuePair<string, ICancellableObserver<TValue>>(key, ret));

            return ret;
        }

        public ICancellableObserver<TValue> FindOrCreate(string key)
        {
            var ret = Find(a => string.Compare(key, a.Key, true) == 0);

            return ret.Value ?? CreateObserver(key);
        }

        public void RemoveObserver(string key)
        {
            RemoveAll(a => string.Compare(key, a.Key, true) == 0);
        }

        //public IObservable<TValue> AddTopic(string topic)
        //{
        //    var target = Find(a => string.Compare(a.Key, topic, true) == 0);

        //    if (target != null)
        //        return target;
        //}
    }
}
