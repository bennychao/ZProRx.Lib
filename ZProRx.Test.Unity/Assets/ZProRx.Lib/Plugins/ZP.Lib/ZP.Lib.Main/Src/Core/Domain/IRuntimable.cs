using System;
namespace ZP.Lib
{
    public interface IRuntimable
    {
        object CurValue { set; get; }
    }

    public interface IRuntimable<T>
    {
        T CurValue { set; get; }
    }
}
