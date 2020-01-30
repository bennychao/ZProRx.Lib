using System;
namespace ZP.Lib
{
    public interface ICalculable<T>
    {
        T Add(T b);
        T Sub(T b);

        //T Add(int b);
        //T Sub(int b);
    }
}
