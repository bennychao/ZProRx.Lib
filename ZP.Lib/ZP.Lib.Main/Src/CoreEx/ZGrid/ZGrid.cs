using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace ZP.Lib{

	public partial class ZGrid<T> : IZGrid, IZGrid<T>, IZGridIterator, IZGridIterator<T>{

		/// <summary>
		/// The data.
		/// </summary>
		ZGridIterator<T> _Data;
		ZGridIterator _Data2;

		ZRowsIterator<T> _Rows;
		ZRowsIterator _Rows2;

		ZRowsIterator<T> _Cols;
		ZRowsIterator _Cols2;

		internal Dictionary<int, T> _DataList;// = new List<T>();

		//Interface's property
		public IZGridIterator<T> Datas {
			get{
				return _Data;
			}
		}

		IZGridIterator IZGrid.Datas {
			get{
				return _Data2;
			}
		}

		public IZRowsIterator<T> Rows {
			get{
				return _Rows;
			}
		}

		IZRowsIterator IZGrid.Rows {
			get{
				return _Rows2;
			}
		}

        public int OriginX { get; set; }
        public int OriginY { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }


		public ZGrid(int witdh, int height){
			_Data = new ZGridIterator<T> (this);
			_Data2 = new ZGridIterator (this);

			_Rows = new ZRowsIterator<T> (this);
			_Rows2 = new ZRowsIterator (this);

			Width = witdh;
			Height = height;

            OriginX = OriginY = 0;

            //init the data list`               
            _DataList = new Dictionary<int, T> (Width * Height);
		}

        //row, col is [0, Width]
		public T this [int row, int col] {
			set {
				SetData (row, col, value);
			}
			get {
				return GetData(row, col);
			}
		}

		public IZRowIterator<T> this [int row] {

			get {
				return Rows[row];
			}
		}

		object IZGridIterator.this [int row, int col] {
			set {
				SetData (row, col, (T)value);
			}
			get {
				return GetData(row, col);
			}
		}

		IZRowIterator IZGridIterator.this [int row] {

			get {
				return Rows[row] as IZRowIterator;
			}
		}

		//Interface's function
		object IZGrid.GetData (int row, int col)
		{
			return GetData(row, col);
		}


		public T GetData (int row, int col)
		{
			int index = col * Width + row;
			if (!_DataList.ContainsKey (index))
				return default(T);

			return _DataList[index];
		}


		void IZGrid.SetData(int row, int col, object data){
			_SetData (row, col, (T)data);
		}


		public void SetData(int row, int col, T data){
			_SetData (row, col, data);
		}


		void _SetData(int row, int col, T data){
			_DataList [col * Width + row] = data;

			//if (typeof(T).IsSubclassOf (ZGridIndexable<T>)) {

			//}
			var indexableData = data as IZGridIndexable;
			if (indexableData != null) {
				indexableData.Row = row;
				indexableData.Col = col;
			}
		}

        public void DeleteData(int row, int col)
        {
            _DataList.Remove(col * Width + row);
        }

        public T GetCenter()
        {
            return GetData(Width / 2, Height / 2);
        }

        /// <summary>
        /// Clear this instance.
        /// </summary>
        public void Clear(){
			_DataList.Clear ();
		}

		bool IZGrid.Contains(object value)
		{
			return _DataList.ContainsValue((T)value);
		}

		public bool Contains(T value)
		{
			return _DataList.ContainsValue (value);
		}

        public bool Contains(int x, int y)
        {
            var key = (y - OriginY) * Width + (x - OriginX);
            return _DataList.ContainsKey(key);
        }

        public T Find(Func<T, bool> comparer){
			foreach (var f in _DataList.Values) {
				if (comparer (f))
					return f;
			}

			return default(T);
		}

		public void Rotate (float a)
		{
			if (!IsOdd ())
				return;

			var newList = new Dictionary<int, T> (Width * Height);

#if !ZP_SERVER
            Matrix4x4 m = Matrix4x4.Rotate(Quaternion.AngleAxis(a, Vector3.forward));
#else
            Matrix4x4 m = Matrix4x4.Rotate(ZTransformEx.AngleAxisEx(a, Vector3.forward));
#endif

            int centerX = Width / 2;
			int centerY = Height / 2;

			foreach (var kv in _DataList) {
				int x = kv.Key % Width; 
				int y = kv.Key / Width;

				Vector3 temp = m.MultiplyPoint( new Vector3(x - centerX, y - centerY));

				int newX = Mathf.RoundToInt( temp.x + centerX);
				int newY = Mathf.RoundToInt(temp.y + centerY);


				var data = GetData(x, y);
				newList [newY * Width + newX] = data;

				//if (typeof(T).IsSubclassOf (ZGridIndexable<T>)) {

				//}
				var indexableData = data as IZGridIndexable;
				if (indexableData != null) {
					indexableData.Row = newX + OriginX;
					indexableData.Col = newY + OriginY;
				}
			}

			_DataList = newList;
		}


		public  void Translate (int offsetX, int offsetY)
		{
            //if (typeof(T).IsSubclassOf (ZGridIndexable<T>)) {

            //}
             OriginX += offsetX;
             OriginY += offsetY;
            foreach (var kv in _DataList)
            {
              int x = kv.Key % Width; 
              int y = kv.Key / Width;
                var data = GetData(x, y);

                var indexableData = data as IZGridIndexable;
                if (indexableData != null)
                {
                    indexableData.Row = x + OriginX;
                    indexableData.Col = y + OriginY;
                }
            }

            //var newList = new Dictionary<int, T> (Width * Height);

            //Matrix4x4 m = Matrix4x4.Translate (new Vector3 (offsetX, offsetY));




            //         int centerX = Width / 2;
            //int centerY = Height / 2;

            //foreach (var kv in _DataList) {
            //	int x = kv.Key % Width; 
            //	int y = kv.Key / Width;

            //	Vector3 temp = m.MultiplyPoint( new Vector3(x - centerX, y - centerY));

            //	int newX = Mathf.RoundToInt( temp.x + centerX);
            //	int newY = Mathf.RoundToInt(temp.y + centerY);


            //	var data = GetData(x, y);
            //	newList [newY * Width + newX] = data;

            //	//if (typeof(T).IsSubclassOf (ZGridIndexable<T>)) {

            //	//}
            //	var indexableData = data as IZGridIndexable;
            //	if (indexableData != null) {
            //		indexableData.Row = newX;
            //		indexableData.Col = newY;
            //	}
            //}

            //_DataList = newList;
        }


        /// <summary>
        /// Identity this instance.从0， 0开始
        /// </summary>
        public void Identity(){
		}

		public bool IsOdd(){
			return Width % 2 != 0;
		}


		public ZGrid<T> SubGrid(int row, int col, int width, int height){
			if (row + height >= this.Height)
				return null;

			if (col + width >= this.Width)
				return null;

			ZGrid<T> ret = new ZGrid<T> (width, height);


			for (int i = 0; i < height; i++) {
				for (int j = 0; j < width; j++) {
					ret.Datas [i, j] = Datas [i + row, j + col];
				}
			}

			return ret;
		}



		public ZGrid<T> DeepClone(){
			ZGrid<T> grid = new ZGrid<T> (this.Width, this.Height);

			foreach (var kv in _DataList) {

				int x = kv.Key % Width; 
				int y = kv.Key / Width;

				grid [x, y] = kv.Value;
			}

			return grid;
		}

		/// <summary>
		/// Resets the index.
		/// </summary>
		public void ResetIndex(){
			foreach (var kv in _DataList) {

				int x = kv.Key % Width; 
				int y = kv.Key / Width;

				var indexableData = kv.Value as IZGridIndexable;
				if (indexableData != null) {
					indexableData.Row = x + OriginX;
					indexableData.Col = y + OriginY;
				}
			}
		}




        

		IEnumerator<T> IEnumerable<T>.GetEnumerator(){
			return _DataList.Values.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator(){
			return _DataList.Values.GetEnumerator ();
		}

		IEnumerator IZGrid.GetRowEnumerator()
		{
			return new ZRowsEnumerator<T> (this);
		}


		IEnumerator<IZRowIterator<T>> IZGrid<T>.GetRowEnumerator()
		{
			return new ZRowsEnumerator<T> (this);
		}

		IEnumerator IZGrid.GetColEnumerator()
		{
			return new ZRowsEnumerator<T> (this);
		}

		IEnumerator<IZRowIterator<T>> IZGrid<T>.GetColEnumerator()
		{
			return new ZRowsEnumerator<T> (this);
		}


		/// <summary>
		/// Gets the row.TODOneed use the objects pool
		/// </summary>
		IZRowIterator IZGrid.GetRow (int row)
		{
			return new ZRowIterator<T>(this, row);
		}


		IZRowIterator IZGrid.GetCol (int col)
		{
			return new ZRowIterator<T>(this, col);
		}


		public IZRowIterator<T> GetRow (int row)
		{
			return new ZRowIterator<T>(this, row);
		}


		public IZRowIterator<T> GetCol (int col)
		{
			return new ZRowIterator<T>(this, col);
		}

        //public void DeleteRow(int index)
        //{

        //}
    }
}


