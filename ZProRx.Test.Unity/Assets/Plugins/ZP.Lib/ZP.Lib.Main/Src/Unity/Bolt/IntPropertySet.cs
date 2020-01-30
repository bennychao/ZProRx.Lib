using System;
#if ZP_BOLT
using Bolt;

namespace ZP.Lib
{
	[UnitCategory ("ZProperty"), UnitOrder (100), UnitShortTitle ("Int Property Setter")]
	public class IntPropertySet : PropertySetterUnit<int>
	{
		public IntPropertySet ()
		{
		}
	}
}

#endif