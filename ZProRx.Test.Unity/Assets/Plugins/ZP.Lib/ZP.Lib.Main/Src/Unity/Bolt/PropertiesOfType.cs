using System;
#if ZP_BOLT
using Bolt;
using Ludiq;
using System.Collections.Generic;

namespace ZP.Lib
{
	[UnitCategory ("ZProperty"), UnitOrder (10)]
	public class PropertiesOfType : Unit
	{
		public PropertiesOfType ()
		{
		}

		private Type _Type;

		[Inspectable, Serialize, TypeFilter (new Type[] {
		}, Enums = false, Classes = true, Interfaces = false, Structs = false, Primitives = false), UnitHeaderInspectable]
		public Type Type {
			get{
				return _Type;
			}

			set{
				_Type = value;
				InitPropertyIDs (value);

			}
		}


		//
		// Methods
		//
		protected override void Definition (){


//			enter = new Bolt.ControlInput (this, "1", Action);
//			this.controlInputs.Add (enter);
//			exit = new Bolt.ControlOutput (this, "2");
//			this.controlOutputs.Add (exit);
//
//			a = new Bolt.ValueInput(this, "A", typeof(int));
//			b = new Bolt.ValueInput(this, "B", typeof(int));
//			this.valueInputs.Add (a);
//			this.valueInputs.Add (b);
//
//			ValueInput c = new Bolt.ValueInput (this, "C", typeof(int));
//			this.valueInputs.Add (c);
//
//			result = new Bolt.ValueOutput (this, "result", typeof(int), Operation);
//
//
//
//			Bolt.UnitRelation r = new UnitRelation (enter, exit);
//
//
//			this.relations.Add (r);
//
//			this.valueOutputs.Add (result);
			//base.Definition ();
		}

		private void InitPropertyIDs(Type type){
			this.valueOutputs.Clear ();

			List<string> rets = ZPropertyMesh.GetPropertyIDs (type);

			foreach (var t in rets) {
				ValueOutput c = new Bolt.ValueOutput (this, t, typeof(string), _=> t);
				this.valueOutputs.Add (c);
			}


		}
	}
}

#endif