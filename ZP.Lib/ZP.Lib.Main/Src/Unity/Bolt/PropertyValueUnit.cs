using System;
#if ZP_BOLT
using Bolt;
using Ludiq;
using UnityEngine;
using UnityEngine.Assertions;
using UniRx;

namespace ZP.Lib
{
	
	public class PropertyValueUnit<T> : Bolt.Event
	{
		public PropertyValueUnit ()
		{
		}

		[PortLabel ("PropertyID"), DoNotSerialize]
		public ValueInput ID {
			get;
			private set;
		}


		[PortLabel ("Cur Value"), DoNotSerialize]
		public ValueOutput CurValue {
			get;
			private set;
		}

		protected override void Definition (){

			base.Definition ();
			ID = new Bolt.ValueInput (this, "PropertyID", typeof(string));
			this.valueInputs.Add (ID);

			CurValue = new Bolt.ValueOutput (this, "Cur Value", typeof(T), GetCurValue);
			this.valueOutputs.Add (CurValue);


			Bolt.UnitRelation r = new UnitRelation (ID, CurValue);
			this.relations.Add (r);

			Bolt.UnitRelation r1 = new UnitRelation (ID, base.trigger);

			this.relations.Add (r1);


		}


		public override void StartListening ()
		{
			base.StartListening ();

			Observable.NextFrame ().Subscribe (_ => RegisterChanged ());

		}

		private void RegisterChanged(){

			Transform trans = (base.owner.GetComponent<MonoBehaviour> () as MonoBehaviour).transform;
			IZProperty prop = ZViewBuildTools.GetPropertyInSubs (trans, ID.GetValue<string>());

			Assert.IsNotNull (prop, "not this property " + ID.GetValue<string>());
			if (prop != null) {
				prop.OnValueChanged += a => {
					//changedValue = (T)a;
					base.Trigger();
				};
			}
		}

		public override void StopListening ()
		{
			base.StopListening ();
		}

		/// <summary>
		/// Operation the specified recursion.
		/// </summary>
		/// <param name="recursion">Recursion.</param>
		public object GetCurValue (Recursion recursion)
		{
			Transform trans = (base.owner.GetComponent<MonoBehaviour> () as MonoBehaviour).transform;
			IZProperty prop = ZViewBuildTools.GetPropertyInSubs (trans, ID.GetValue<string>());

			Assert.IsNotNull (prop, "not this property " + ID.GetValue<string>());
			if (prop != null) {
				return prop.Value;
			}

			return default(T);
		}

	}
}

#endif