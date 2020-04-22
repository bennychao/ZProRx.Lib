using System;
using ZP.Lib;
using UnityEngine;
using ZP.Lib.Common;

namespace ZP.Lib
{

	/// <summary>
	/// Z layout2这里需要说明一下，方向是X/Y方向，从0.0开始，所有飞机头在大数位置。
	/// </summary>
	public class ZLayout2<T>
	{
//		public class ZLayoutTransform<T>{
//			public ZProperty<int> x = new ZProperty<int>();
//			public ZProperty<int> y = new ZProperty<int>();
//		}

		public ZProperty<int> Width = new ZProperty<int> ();
		public ZProperty<int> Height = new ZProperty<int>();

		public ZPropertyList<T> Items = new ZPropertyList<T>();

		public ZProperty<int> Count = new ZProperty<int>();

		public ZGrid<T> Grid;// = new ZGrid<ShipAxesItem>();


		public ZLayout2 ()
		{


		}

        /// <summary>
        /// Raises the create event.
        /// </summary>
        public void OnCreate(){
            //Count.Select<ZPropertyList<T >> (Items, (items) => items.Count);
            Count.Count(Items);
		}

        /// <summary>
        /// Ons the load.
        /// </summary>
		public void OnLoad(){
			Init ();
		}

        public void OnCopy()
        {
            Init();
        }

        public void OnBind(Transform layoutRoot){
			//Debug.Log ("");
		}


		public void Init(){
			Grid = new ZGrid<T> (Width, Height);

			foreach (var i in Items) {
				var indexor = i as IZGridIndexable;
				if (indexor == null)
					continue;
				
				Grid [indexor.Row, indexor.Col] = i;
			}
		}

        public void InitOne()
        {
            Items.ClearAll();
            Width.Value = 1;
            Height.Value = 1;

            if (Grid == null)
                Grid = new ZGrid<T>(Width, Height);
            else
                Grid.Clear();


            Grid.Width = Width.Value;
            Grid.Height = Height.Value;


            InsertItem(Vector2.zero);

            Grid.ResetOrigin(0, 0);
            Grid.ResetStruct();
        }

        public void InitWithRect(Rect rect)
        {
            Items.ClearAll();
            //count will auto update

            var ps = rect.IntPoints();
            var size = rect.IntSize();

            Width.Value = (int)size.x;
            Height.Value = (int)size.y;

            if (Grid == null)
                Grid = new ZGrid<T>(Width, Height);
            else
                Grid.Clear();

            Grid.Width = Width.Value;
            Grid.Height = Height.Value;

            foreach (var it in ps)
            {
                InsertItem(it);
            }

            Grid.ResetOrigin((int)rect.min.x, (int)rect.min.y);
            Grid.ResetStruct();
        }

        /// <summary>
        /// Clone this instance.
        /// </summary>
        public ZLayout2<T> Clone(){
			ZLayout2<T> ret = ZPropertyMesh.CreateObject< ZLayout2<T> >();
			//ret = this;
			ret.Width = this.Width;
			ret.Height = this.Height;
			ret.Items = this.Items;
			ret.Count = this.Count;

			ret.Grid = this.Grid.DeepClone ();

			return ret;
		}



		/// <summary>
		/// Moves to center.
		/// </summary>
		public void MoveToCenter(){

			Grid.Translate (-Width.Value / 2, -Height.Value / 2);
		}

		public Vector2 GetCenter(){
			return new Vector2(Width.Value / 2, Height.Value / 2);
		}

        public void TranslateWindows(int offsetX, int offsetY)
        {
            if (offsetX > 0)
            {
                //delete row 0{
                //Grid.SetData(0, 0, def);

            }
        }

        /// <summary>
        /// Reset this instance.
        /// </summary>
        public void Reset(){
			this.Grid.ResetIndex ();
		}


        protected void InsertItem(Vector2 pos)
        {
            var item = ZPropertyMesh.CreateObject<T>();
            if (item as IZGridIndexable != null)
            {
                (item as IZGridIndexable).Row = (int)pos.x;
                (item as IZGridIndexable).Col = (int)pos.y;
            }

            Grid.Insert((int)pos.x, (int)pos.y, item);

            Items.Add(item);
        }


        protected void DeleteItem(Vector2 pos)
        {
           // var item = ZPropertyMesh.CreateObject<T>();

            Grid.Delete((int)pos.x, (int)pos.y);

            //will updatet he views and links
            Items.Remove(a =>
            {
                var indexr = a as IZGridIndexable;
                if (indexr == null)
                    return false;

                return indexr.Col == (int)pos.y && indexr.Row == (int)pos.x;
            });

        }



        //switch to rect
        public void Relayout(Rect rect)
        {
            var curRect = Grid.ToRect();

            //Zoom out
            if (rect.Contains(curRect))
            {
                //add new items
                var moreRects = curRect.GetExclusiveInside(rect);
                foreach (var re in moreRects)
                {
                    var ps = re.IntPoints();
                    foreach (var it in ps)
                    {
                        InsertItem(it);
                    }
                }

                Grid.ResetOrigin((int)rect.min.x, (int)rect.min.y);
                Grid.ResetStruct();
            }
            else if (curRect.Contains(rect))
            {
                //delete items
                var rects = curRect.GetExclusiveInside(rect);
                foreach (var re in rects)
                {
                    var ps = re.IntPoints();
                    foreach (var it in ps)
                    {
                        DeleteItem(it);
                    }
                }

                Grid.ResetOrigin((int)rect.min.x, (int)rect.min.y);
                Grid.ResetStruct();
            }
            else if (curRect.IsIntersect(rect))
            {
                var delRects = curRect.GetExclusiveIntersect(rect);
                foreach (var re in delRects)
                {
                    var ps = re.IntPoints();
                    foreach (var it in ps)
                    {
                        DeleteItem(it);
                    }
                }

                var addrects = rect.GetExclusiveInside(curRect);
                foreach (var re in addrects)
                {
                    var ps = re.IntPoints();
                    foreach (var it in ps)
                    {
                        InsertItem(it);
                    }
                }

                Grid.ResetOrigin((int)rect.min.x, (int)rect.min.y);
                Grid.ResetStruct();
            }
        }//Relayout end

        public IObservable<T> CellsEventObservable(string propID)
        {
            return new ListSubEventObservable<T>(Items, propID);
        }
    }
}

