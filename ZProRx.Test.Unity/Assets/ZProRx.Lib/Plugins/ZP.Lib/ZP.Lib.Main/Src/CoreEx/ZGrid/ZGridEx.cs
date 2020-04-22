using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZP.Lib
{
    public partial class ZGrid<T> : IZGrid, IZGrid<T>, IZGridIterator, IZGridIterator<T>
    {

        /// <summary>
        /// Debugs the show.
        /// </summary>
        public void DebugShow()
        {
            //Debug.Log("=========");
            foreach (var kv in _DataList)
            {

                //Debug.Log("kv " + kv.Key.ToString() + " v= " + kv.Value.ToString() + "\n");
            }
        }


        public Rect ToRect() {
            return new Rect((float)OriginX - 0.5f, (float)OriginY - 0.5f, 
            (float)Width + 1, (float)Height + 1);
        }

        //int the rect
        public bool InRange(int x, int y)
        {
            var rect = ToRect();
            return rect.Contains(new Vector2((float)x, (float)y));
        }




        public void Insert(int x, int y, T item)
        {
            _DataList.Add(y * Width + x, item);
        }

        public void Delete(int x, int y)
        {
            _DataList.Remove(y * Width + x);
        }

        //reinit the data list by IZGridIndexable
        public void ResetStruct()
        {
            var newList = new Dictionary<int, T>(Width * Height);

            //find Origin

            foreach (var v in _DataList)
            {
                var indexr = v.Value as IZGridIndexable;
                if (indexr == null)
                {
                    throw new Exception("only indexable value support reset struct");
                    //return;
                }
                //_DataList[col * Width + row] = data;
                var key = (indexr.Col - OriginY) * Width + indexr.Row - OriginX;

                newList[key] = v.Value;
            }

            //newList.
        }

        public void ResetOrigin(int x, int y)
        {
            OriginY = y;
            OriginX = x;
        }

    }
}
