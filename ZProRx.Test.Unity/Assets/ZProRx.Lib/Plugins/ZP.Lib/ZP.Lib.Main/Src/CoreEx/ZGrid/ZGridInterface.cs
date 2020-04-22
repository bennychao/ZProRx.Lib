using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
	public interface IZGridIterator : IEnumerable{
		//int x{ set; get; }
		object this [int row, int col]{ set; get;}

		IZRowIterator this [int row]{ get;}
	}

	public interface IZGridIterator<T>: IEnumerable, IEnumerable<T>{
		//int x{ set; get; }
		T this [int row, int col]{ set; get;}

		IZRowIterator<T> this [int row]{ get;}
	}

	public interface IZRowsIterator: IEnumerable{
		IZRowIterator this [int index]{ get;}
	}

	public interface IZRowsIterator<T>: IEnumerable<IZRowIterator<T> >, IEnumerable{
		IZRowIterator<T> this [int index]{ get;}
	}

	public interface IZRowIterator: IEnumerable{
		int cur{ set; get;}
	}

	public interface IZRowIterator<T>: IEnumerable<T>, IEnumerable{
		int cur{ set; get;}
	}


	public interface IZGrid : IEnumerable{
		int Width{ get; }
		int Height{ get;}

		IZGridIterator Datas { get;}

		IZRowsIterator Rows{ get;}

		object GetData (int row, int col);
		void SetData (int row, int col, object data);

		IZRowIterator GetRow (int row);
		IZRowIterator GetCol (int col);


		void Clear();
		bool Contains(object value);
		void Rotate (float a);

		IEnumerator GetRowEnumerator();
		IEnumerator GetColEnumerator();
	}

	public interface IZGrid<T> : IEnumerable<T>{
		int Width{ get; }
		int Height{ get;}

		T GetData (int row, int col);
		void SetData (int row, int col, T data);

        T GetCenter();

		IZGridIterator<T> Datas { get;}

		IZRowsIterator<T> Rows{ get;}

		void Clear();
		bool Contains(T value);
		void Rotate (float a);

		IZRowIterator<T> GetRow (int row);
		IZRowIterator<T> GetCol (int col);

		IEnumerator<IZRowIterator<T>> GetRowEnumerator();
		IEnumerator<IZRowIterator<T>> GetColEnumerator();
	}
}

