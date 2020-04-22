using System;
using System.Collections.Generic;

namespace ZP.Lib
{
	[PropertyRuntimeClassAttribute]
    public partial class ZRuntimableProperty<T> : ZProperty<T>, IRuntimable, IRuntimable<T>, IRecoverable
	{
		protected T cur = default(T);

		protected List<KeyValuePair<uint, T> > records = null;

		/// <summary>
		/// Gets or sets the current value.
		/// </summary>
		/// <value>The current value.</value>
		object IRuntimable.CurValue{ 
			set { 
				setCurValue((T)value);
			} 
			get{ 
				return (object)cur;
			}
		}

		/// <summary>
		/// Gets or sets the current value.
		/// </summary>
		/// <value>The current value.</value>
		public T CurValue{ 
			set { 
				setCurValue((T)value);
			} 
			get{ 
				return cur;
			}
		}

        public void ToDefault()
        {
            if (ZPropertyAttributeTools.IsDefaultToValue(this))
                CurValue = Value;
            else
                CurValue = default(T);
        }

        /// <summary>
        /// Inners the init.
        /// </summary>
        public void  OnLoad()
		{
			var attr = ZPropertyAttributeTools.GetAttribute<PropertyDefaultToValueAttribute> (this as IZProperty);
			if (attr != null) {
				setCurValue (Value);
			}
		}


		/// <summary>
		/// Clone the specified prop.
		/// </summary>
		/// <param name="prop">Property.</param>
		public override void CopyData(IZProperty prop){
            base.CopyData(prop);
			base.data = (prop as ZRuntimableProperty<T>).data;
			this.cur = (prop as ZRuntimableProperty<T>).cur;
		}


		/// <summary>
		/// Sets the current value.
		/// </summary>
		/// <param name="value">Value.</param>
		private void setCurValue(T value){
			cur = (T)value;

			var rea = this.AttributeNode.GetAttribute<PropertyRecordAttribute>();
			if (rea != null && rea.bSupport) {
				if (records == null) {
					records = new List<KeyValuePair<uint, T>> ();
				}

				records.Add (new KeyValuePair<uint, T> (GetTimestamp (), cur));
				if (records.Count > rea.maxRecordCount) {
					records.RemoveAt (0);
				}
			}


            SendChange(cur);
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="value">Value.</param>
		override protected void SetValue(T value){
			base.SetValue (value);
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		protected override T GetValue ()
		{
			return base.GetValue ();
		}

		/// <summary>
		/// Gets the current time.
		/// </summary>
		/// <returns>The current time.</returns>
		public static uint GetCurTime(){
			return GetTimestamp ();
		}

		/// <summary>
		/// Redos to time.返回到参数时间前最后一个记录
		/// </summary>
		/// <param name="time">Time.</param>
		public T RedoToTime(uint time){

			if (records == null || records.Count <= 0)
				return cur;
			
			var v = records.FindLastIndex (a => a.Key <= time);

			if (v == -1)
				return cur;
			
			cur = records[v].Value;
			return cur;
		}

		/// <summary>
		/// Redos to time.
		/// </summary>
		/// <returns>The to time.</returns>
		/// <param name="time">Time.</param>
		object IRecoverable.RedoToTime(uint time){

			if (records == null || records.Count <= 0)
				return cur;

			var v = records.FindLastIndex (a => a.Key <= time);

			if (v == -1)
				return cur;

			cur = records[v].Value;
			return cur;
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		void IRecoverable.Reset(){
			records.Clear ();
		}

		/// <summary>
		/// Gets the timestamp.
		/// </summary>
		/// <returns>The timestamp.</returns>
		public static uint GetTimestamp()
		{
			TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
			return (uint)ts.TotalMilliseconds;
		}
	}
}

