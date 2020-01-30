using System;
#if ZP_BOLT
using Bolt;
using Ludiq;
using UnityEngine;
using UnityEngine.Assertions;

namespace ZP.Lib
{
	public class PropertySetterUnit<T> : Unit
	{
		[Inspectable, Serialize, UnitHeaderInspectable("Is Runtime")]
		public bool bRuntime {
			set;
			get;
		}


		[PortLabel ("PropertyID"), DoNotSerialize]
		public ValueInput ID {
			get;
			private set;
		}

		[DoNotSerialize]
		public ValueInput Value {
			get;
			set;
		}


		[PortLabelHidden, DoNotSerialize]
		public ControlInput enter {
			get;
			private set;
		}

		[PortLabelHidden, DoNotSerialize]
		public ControlOutput exit {
			get;
			private set;
		}

		public PropertySetterUnit ()
		{
		}

		//
		// Methods
		//
		protected override void Definition (){

			enter = new Bolt.ControlInput (this, "Set", Action);
			this.controlInputs.Add (enter);
			exit = new Bolt.ControlOutput (this, "Out");
			this.controlOutputs.Add (exit);

			Value = new Bolt.ValueInput (this, "Value", typeof(T));
			this.valueInputs.Add (Value);


			Bolt.UnitRelation r = new UnitRelation (enter, exit);
			this.relations.Add (r);

			Bolt.UnitRelation r1 = new UnitRelation (Value, exit);
			this.relations.Add (r1);
			//base.Definition ();
		}
			

		public void Action(Flow flow){

			Transform trans = (base.owner.GetComponent<MonoBehaviour> () as MonoBehaviour).transform;
			IZProperty prop = ZViewBuildTools.GetPropertyInSubs (trans, ID.GetValue<string>());

			Assert.IsNotNull (prop, "not this property " + ID.GetValue<string>());
			if (prop != null) {

				if (bRuntime)
					(prop as IRuntimable).CurValue = Value.GetValue<T> ();
				else
					prop.Value = Value.GetValue<T> ();
			}

			flow.Invoke (exit);
		}


	}
}
#endif
