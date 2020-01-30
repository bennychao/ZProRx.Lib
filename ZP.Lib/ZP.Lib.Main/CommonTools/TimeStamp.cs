using System;

namespace ZP.Lib.Common
{
	public class TimeStamp
	{
		public static uint GetTimestamp()
		{
			TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
			return (uint)ts.TotalMilliseconds;
		}
	}
}

