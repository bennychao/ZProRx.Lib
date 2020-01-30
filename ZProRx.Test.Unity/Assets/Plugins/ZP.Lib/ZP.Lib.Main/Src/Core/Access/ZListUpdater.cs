using System;
using System.Collections.Generic;

namespace ZP.Lib
{
    public class ZListUpdater
    {
        public ZPropertyList<UpdateItem> Items = new ZPropertyList<UpdateItem>();

        public void Add(UpdateItem item)
        {
            Items.Add(item);
        }

        public void AddRange(List<UpdateItem> item)
        {
            Items.AddRange(item);
        }
        public void Merge(ZListUpdater other)
        {
            Items.AddRange(other.Items);
        }
    }
}
