using System;
using ZP.Lib;

namespace ZP.Lib.Card
{

    public interface IZGroup{

    }

    public interface IZGroup<T> : IZGroup
    {

    }

    public class ZGroup<T> : IZGroup, IZGroup<T>
    {
        public ZProperty<int> CountryID = new ZProperty<int>();

        public ZPropertyRefList<IZGroup> Link = new ZPropertyRefList<IZGroup>();


        public ZGroup()
        {
        }


    }
}
