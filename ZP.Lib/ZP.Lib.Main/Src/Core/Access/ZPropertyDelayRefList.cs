using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ZP_UNIRX
using UniRx;
using System.Linq;
using System;

namespace ZP.Lib
{
    public class ZPropertyDelayRefList<T> : ZPropertyRefList<T>
    {
        public ZPropertyDelayRefList()
        {
            this.OnBindProp = null;
        }

        protected T DelayBind(int id)
        {
            var attr = this.AttributeNode.GetAttribute<PropertyResAttribute>();
            if (attr == null)
                return default(T);

            Dictionary<string, string> queryParam = new Dictionary<string, string>();
            queryParam["id"] = id.ToString();
            queryParam["Action"] = "Query";

            //get net
            ZPropertyNet.Load<ZListUpdater>(attr.Path, queryParam).Subscribe(a =>
            {
                ZPropertyUpdater.UpdateObject(this, a);
            });

            return default(T);
        }

        public void Queries()
        {
            var attr = this.AttributeNode.GetAttribute<PropertyResAttribute>();
            if (attr == null)
                return;

            var ids = PropList.Where(a => a.Value != null).Select(a => (a as IRefable)?.RefID).ToArray();

            if (ids == null || ids.Length <= 0)
            {
                return;
            }

            Dictionary<string, string> queryParam = new Dictionary<string, string>();
            queryParam["ids"] = ids.ToString();
            queryParam["Action"] = "Queries";

            //get net
            ZPropertyNet.Load<ZListUpdater>(attr.Path, queryParam).Subscribe(a =>
            {
                ZPropertyUpdater.UpdateObject(this, a);
            });
        }

        public void Queries(params int[] ids)
        {
            var attr = this.AttributeNode.GetAttribute<PropertyResAttribute>();
            if (attr == null)
                return;


            if (ids == null || ids.Length <= 0)
            {
                return;
            }

            Dictionary<string, string> queryParam = new Dictionary<string, string>();
            queryParam["ids"] = ids.ToString();
            queryParam["Action"] = "Queries";

            //get net
            ZPropertyNet.Load<ZListUpdater>(attr.Path, queryParam).Subscribe(a =>
            {
                ZPropertyUpdater.UpdateObject(this, a);
            });
        }

        public void QueryOne(int id, Action<T> queryEnd  = null)
        {
            var attr = this.AttributeNode.GetAttribute<PropertyResAttribute>();
            if (attr == null)
                return;


            Dictionary<string, string> queryParam = new Dictionary<string, string>();
            queryParam["id"] = id.ToString();
            queryParam["Action"] = "Query";

            //get net
            ZPropertyNet.Load<ZListUpdater>(attr.Path, queryParam).Subscribe(a =>
            {
                ZPropertyUpdater.UpdateObject(this, a);
                if (queryEnd != null)
                {
                    queryEnd( GetValue(id));
                }
            });
        }

        public T TryGetValue(int id, Action<T> queryEnd = null)
        {
            var prop = FindProperty(a => a == id);
            if (prop != null && prop.Value != null)
            {
                if (queryEnd != null)
                    queryEnd((T)prop.Value);
                return (T)prop.Value;
            }


            QueryOne(id, queryEnd);

            return default(T);
        }
    }
}

#endif