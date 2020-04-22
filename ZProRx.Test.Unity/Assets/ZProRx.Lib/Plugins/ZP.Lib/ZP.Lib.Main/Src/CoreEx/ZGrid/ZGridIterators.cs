using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
	/// <summary>
	/// Z grid data index class with template.
	/// </summary>
	public class ZGridIterator<T> : IZGridIterator<T>{
		private ZGrid<T> _grid;

		internal ZGridIterator(ZGrid<T> grid){
			_grid = grid;
		}

		public T this [int row, int col] {
			set {
				_grid.SetData (row, col, value);
			}
			get {
				return _grid.GetData(row, col);
			}
		}

		public IZRowIterator<T> this [int row] {

			get {
				return _grid.Rows[row];
			}
		}

		public IEnumerator GetEnumerator(){
			return _grid._DataList.Values.GetEnumerator ();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator(){
			return ((IEnumerable<T>)_grid._DataList.Values).GetEnumerator();
		}
	}

	/// <summary>
	/// Z grid data index class .
	/// </summary>
	public class ZGridIterator : IZGridIterator{
		private IZGrid _grid;

		internal ZGridIterator(IZGrid grid){
			_grid = grid;
		}

		public object this [int row, int col] {
			set {
				_grid.SetData (row, col, value);
			}
			get {
				return (object)(_grid.GetData(row, col));
			}
		}

		public IZRowIterator this [int row] {

			get {
				return (IZRowIterator)(_grid.Rows[row]);
			}
		}

		public IEnumerator GetEnumerator(){
			return _grid.GetEnumerator();
		}
	}



	/// <summary>
	/// Z row iterator.
	/// </summary>
	public class ZRowIterator<T> : IZRowIterator, IZRowIterator<T>{

		private ZGrid<T> _grid;

		public int cur{ set; get;}

		internal ZRowIterator(ZGrid<T> grid, int curRow){
			_grid = grid; 
			cur = curRow;
		}

		IEnumerator  IEnumerable.GetEnumerator(){
			return new ZRowEnumerator<T>(_grid, cur);
		}

		 IEnumerator<T>  IEnumerable<T>.GetEnumerator(){
			return new ZRowEnumerator<T>(_grid, cur);
		}
	}


	/// <summary>
	/// Z row enumerator.
	/// </summary>
	public class ZRowEnumerator<T> : IEnumerator, IEnumerator<T>{
		private ZGrid<T> _grid;

		private int curRow = -1;

		private int curPos = -1;

		internal ZRowEnumerator(ZGrid<T> grid, int curRow){
			_grid = grid; 
			this.curRow = curRow;
		}

		public bool MoveNext()
		{
			if (curPos != _grid.Width) {
				curPos++;
			}

			return curPos < _grid.Width;
		}

		object IEnumerator.Current
		{
			get
			{
				if (curPos < _grid.Width || curPos != -1) {
					return _grid.GetData(curRow, curPos);
				}

				return null;
			}
		}


		public T Current
		{
			get
			{
				if (curPos < _grid.Width || curPos != -1) {
					return _grid.GetData(curRow, curPos);
				}

				return default(T);
			}
		}



		public void Reset()
		{
			curPos = -1;//将游标重置为-1  #7
		}

		public void Dispose()
		{

		}
	}


	/// <summary>
	/// Z rows iterator.
	/// </summary>
	public class ZRowsIterator : IZRowsIterator{
		private IZGrid _grid;

		internal ZRowsIterator(IZGrid grid){
			_grid = grid;
		}

		public IZRowIterator this [int row] {

			get {
				return _grid.GetRow(row);
			}
		}

		public IEnumerator GetEnumerator(){
			return _grid.GetColEnumerator();
		}
	}



	/// <summary>
	/// Z rows iterator with template.
	/// </summary>
	public class ZRowsIterator<T> : IZRowsIterator<T>{
		private ZGrid<T> _grid;

		internal ZRowsIterator(ZGrid<T> grid){
			_grid = grid;
		}

		public IZRowIterator<T> this [int row] {

			get {
				return _grid.GetRow(row);
			}
		}


		public IEnumerator<IZRowIterator<T> > GetEnumerator(){
			return ((IZGrid<T>)_grid).GetColEnumerator() as IEnumerator<IZRowIterator<T>>;
		}

		IEnumerator IEnumerable.GetEnumerator(){
			return ((IZGrid)_grid).GetColEnumerator();;
		}
	}




	/// <summary>
	/// Z rows enumerator.
	/// </summary>
	public class ZRowsEnumerator<T> : IEnumerator, IEnumerator<IZRowIterator<T>>{	//

		private ZGrid<T> _grid;

		private int curRow = -1;

		internal ZRowsEnumerator(ZGrid<T> grid){
			_grid = grid;
		}

		public bool MoveNext()
		{
			if (curRow != _grid.Height) {
				curRow++;
			}

			return curRow < _grid.Height;
		}

		object IEnumerator.Current
		{
			get
			{
				if (curRow < _grid.Height || curRow != -1) {
					return null;
				}

				return null;
			}
		}


		public IZRowIterator<T> Current
		{
			get
			{
				if (curRow < _grid.Height || curRow != -1) {
					return ((IZGrid<T>)_grid).GetRow (curRow);
				}

				return null;
			}
		}



		public void Reset()
		{
			curRow = -1;//将游标重置为-1  #7
		}

		public void Dispose()
		{
			
		}
	}
}

