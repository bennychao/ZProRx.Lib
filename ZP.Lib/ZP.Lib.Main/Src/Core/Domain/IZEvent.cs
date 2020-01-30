using System;

namespace ZP.Lib
{
    public interface IZEvent : IZPropertable
    {
        Action OnEvent { set; get; }
        void Invoke();
    }

    public interface IZEvent< T> : IZEvent
    {
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
