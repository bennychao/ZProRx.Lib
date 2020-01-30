using System;
#if ZP_BOLT
using Bolt;
using Ludiq;
using UnityEngine;
using UnityEngine.Assertions;

namespace ZP.Lib
{
	public class PropertyChangedUnit<T> : Bolt.Event
	{
		public PropertyChangedUnit ()
		{
		}

		[PortLabel ("PropertyID"), DoNotSerialize]
		public ValueInput ID {
			get;
			private set;
		}

		[PortLabel ("Value"), DoNotSerialize]
		public ValueOutput Value {
			get;
			private set;
		}

		private T changedValue = default(T); 

		protected override void Definition (){

			base.Definition ();

			ID = new Bolt.ValueInput (this, "PropertyID", typeof(string));
			this.valueInputs.Add (ID);

			Value = new Bolt.ValueOutput (this, "Value", typeof(T), Operation);
			this.valueOutputs.Add (Value);

			Bolt.UnitRelation r = new UnitRelation (ID, Value);


			this.relations.Add (r);

			Bolt.UnitRelation r1 = new UnitRelation (ID, base.trigger);

			this.relations.Add (r1);


		}

		public override void StartListening ()
		{
			base.StartListening ();

			Transform trans = (base.owner.GetComponent<MonoBehaviour> () as MonoBehaviour).transform;
			IZProperty prop = ZViewBuildTools.GetPropertyInSubs (trans, ID.GetValue<string>());

			Assert.IsNotNull (prop, "not this property " + ID.GetValue<string>());
			if (prop != null) {
				prop.OnValueChanged += a => {
					changedValue = (T)a;
					base.Trigger();
				};
			}

		}

		public override void StopListening ()
		{
			base.StopListening ();
		}


		public object Operation (Recursion recursion)
		{
			return changedValue;
		}
	}
}

#endif