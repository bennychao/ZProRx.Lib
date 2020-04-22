using System;
using UnityEngine;

namespace ZP.Lib.Core
{
    public interface IZShape
    {
        bool Contain(Vector2 pos);

        Vector2 RandomPos();
    }
}