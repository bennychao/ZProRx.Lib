using System;

namespace ZP.Lib
{
    //row is global Origin + local row
    public interface IZGridIndexable{
		int Row{ set; get;} //X
		int Col{ set; get;} //Y
	}
	
	public class ZGridIndexable<T> : IZGridIndexable where T : ZGridIndexable<T>
	{
		public int Row{ set; get; } //X
        public int Col{ set; get; } //Y

        public ZGridIndexable ()
		{
			

		}
	}
}

