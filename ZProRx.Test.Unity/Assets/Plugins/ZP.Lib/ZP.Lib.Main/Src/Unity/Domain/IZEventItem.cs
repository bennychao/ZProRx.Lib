using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZP.Lib
{
    public interface IZEventItem
    {
        bool Bind(IZEvent zEvent);
        void Unbind();
    }

    public interface IZEventItem<T> : IZEventItem
    {
        bool Bind(IZEvent<T> zEvent);
    }
}

