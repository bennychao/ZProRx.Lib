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

		
		public static float Next(float min, float max)
		{
			System.Random ran = new Random();
			float rID = ran.Next((int)(min * 100), (int)(max * 100)) / 100.0f;

			return rID;
		}
	}
}

