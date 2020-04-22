using System;
#if ZP_BOLT
using Bolt;

namespace ZP.Lib
{
	[UnitCategory ("ZProperty"), UnitOrder (2), UnitShortTitle ("Int Property Value")]
	public class PropertyIntValue : PropertyValueUnit<int>
	{
		public PropertyIntValue ()
		{
		}
	}

//	[UnitCategory ("ZProperty"), UnitOrder (3), UnitShortTitle ("float Property Value")]
//	public class PropertyFloatValue : PropertyValueUnit<float>
//	{
//		public PropertyFloatValue ()
//		{
//		}
//	}
//
//	[UnitCategory ("ZProperty"), UnitOrder (4), UnitShortTitle ("bool Property Value")]
//	public class PropertyBoolValue : PropertyValueUnit<bool>
//	{
//		public PropertyBoolValue ()
//		{
//		}
//	}
}

#endif