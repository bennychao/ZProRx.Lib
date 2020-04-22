using System;
#if ZP_BOLT
using Bolt;

namespace ZP.Lib
{
		[UnitCategory ("ZProperty"), UnitOrder (3), UnitShortTitle ("float Property Value")]
		public class PropertyFloatValue : PropertyValueUnit<float>
		{
			public PropertyFloatValue ()
			{
			}
		}
}
#endif

