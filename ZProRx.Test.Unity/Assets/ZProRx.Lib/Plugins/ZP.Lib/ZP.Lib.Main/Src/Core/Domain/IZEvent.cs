using System;

namespace ZP.Lib
{
    public interface IZEvent : IZPropertable
    {
        string SimplePropertyID { get; }
        Action OnEvent { set; get; }
        void Invoke();
    }

    public interface IZEventWithParam {

        object CurValue { get; }
    }

    public interface IZEvent< T> : IZEvent, IZEventWithParam
    {
       new  T CurValue { get; }
        new Action<T> OnEvent { set; get; }
        void Invoke(T data);
    }


    public interface IZEventAction
    {
        void OnEvent(IZEvent e);
    }

    public interface IZEventAction<in T>
    {
        void OnEvent(IZEvent e, T param);
    }
}
