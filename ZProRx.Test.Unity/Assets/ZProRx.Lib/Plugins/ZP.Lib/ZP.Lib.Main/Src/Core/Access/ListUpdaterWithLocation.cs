using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZP.Lib
{
    public class ListUpdaterWithLocation
    {
        public List<KeyValuePair<Vector2, UpdateItem>> Items = new List<KeyValuePair<Vector2, UpdateItem>>();

        public void Add(UpdateItem item, Vector2 pos)
        {
            Items.Add(new KeyValuePair<Vector2, UpdateItem>(pos, item));
        }
        public void Merge(ListUpdaterWithLocation other)
        {
            Items.AddRange(other.Items);
        }

        public ZListUpdater ToUpdater()
        {
            ZListUpdater updater = ZPropertyMesh.CreateObject<ZListUpdater>();

            var list = Items.Select((arg) => arg.Value).ToList();

            updater.Items.AddRange(list);

            return updater;
        }

        public ZListUpdater ToUpdater(Vector2 pos)
        {
            ZListUpdater updater = ZPropertyMesh.CreateObject<ZListUpdater>();

            var list = Items.Where((arg) => Vector2.Distance(pos, arg.Key) < 0.001f)?.Select((arg) => arg.Value).ToList();

            updater.Items.AddRange(list);

            return updater;
        }

        public List<UpdateItem> ToUpdateList(Vector2 pos)
        {
            var list = Items.Where((arg) => Vector2.Distance(pos, arg.Key) < 0.001f)?.Select((arg) => arg.Value).ToList();

            return list;
        }

        public List<UpdateItem> ToUpdateList(Rect rect)
        {
            var list = Items.Where((arg) => rect.Contains(arg.Key))?.Select((arg) => arg.Value).ToList();

            return list;
        }
    }
}
