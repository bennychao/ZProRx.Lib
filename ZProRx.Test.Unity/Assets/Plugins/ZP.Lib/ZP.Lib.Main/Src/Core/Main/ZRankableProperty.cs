using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZP.Lib
{
	public class ZRankableProperty<T> : ZRuntimableProperty<T>, IRankable, IRankable<T>, IEnumerable, IEnumerable<T>
	{
		protected List<ZProperty<T> > propList = new List<ZProperty<T>>();

		//protected int curRank = -1;	//value is 0

		public int Count{
			get{
				return propList.Count;
			}
		}

		/// <summary>
		/// Clone the specified prop.
		/// </summary>
		/// <param name="prop">Property.</param>
		public override void CopyData(IZProperty prop){

            base.CopyData(prop);

			var curProp = prop as ZRankableProperty<T>;
			base.data = curProp.data;
			this.cur = curProp.cur;

			foreach (var p in curProp.propList) {

				ZProperty<T> newProp = new ZProperty<T> ();
				newProp.CopyData (p);

				if (ZPropertyMesh.IsPropertable (p.Value.GetType ())) {
					
					newProp.Value = (T)ZPropertyMesh.CloneObject ((object)(p.Value));
					propList.Add (newProp);
				} else {
					propList.Add (newProp);
				}
			}
		}

		void IRankable.AddRank (object rankData)
		{
			var prop = new ZProperty<T> ((T)rankData);
			ZPropertyMesh.BindPropertyAttribute (prop, PropertyID);
			propList.Add((ZProperty<T>)prop);
		}

		override protected void SetValue(T value){
			base.SetValue (value);
		}

		protected override T GetValue ()
		{
			return base.GetValue ();
		}

		object IRankable.GetRank (int rank){
			return (object)(propList [rank].Value);
		}

		public T GetRank (int rank){

			if (rank < 0)
				return default(T);

			if (rank >= propList.Count)
				rank = propList.Count - 1;

			//int tmp = Math.Min( Math.Max (0, rank), propList.Count - 1);

			return (propList [rank].Value);
		}

		object IRankable.Upgrade (int rank){
			return Upgrade (rank);
		}

		public T Upgrade (int rank){
			if (propList == null || propList.Count <= 0) {
				
				return default(T);
			}

			base.SetValue (GetRank (rank));
			//var oldRankValue = GetRank (curRank);
			base.CurValue = GetRank (rank);// - oldRankValue;

			return base.CurValue;
		}



		IEnumerator IEnumerable.GetEnumerator(){
			return propList.GetEnumerator ();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator(){
			return ((IList<T>)propList).GetEnumerator ();
		}
	
		/// <summary>
		/// Converts to array.
		/// </summary>
		/// <returns>The to array.</returns>
		object IRankable.ConvertToArray (){
			return propList.Select(a => a.Value).ToList<T>();
		}

        public IList<T> ConvertToArray()
        {
            return propList.Select(a => a.Value).ToList<T>();
        }

        //public override string ToString()
        //{
        //    return data.ToString();
        //}
    }
}

