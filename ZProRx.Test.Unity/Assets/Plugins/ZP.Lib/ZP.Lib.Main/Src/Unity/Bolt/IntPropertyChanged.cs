using System;

#if ZP_BOLT

using Bolt;

namespace ZP.Lib
{
	[UnitCategory ("ZProperty"), UnitOrder (1), UnitShortTitle ("Int Property Changed")]
	public class IntPropertyChanged : PropertyChangedUnit<int>
	{
		public IntPropertyChanged ()
		{
		}
	}
}


#endif