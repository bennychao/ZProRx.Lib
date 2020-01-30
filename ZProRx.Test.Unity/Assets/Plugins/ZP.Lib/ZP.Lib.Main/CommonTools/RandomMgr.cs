using System.Collections;
using System.Collections.Generic;
using System;

namespace ZP.Lib{
	public static class RandomMgr{
		public static int Next(int min, int max){
			System.Random ran = new Random();
			int rID = ran.Next(min, max);

			return rID;
		}
	}
}

