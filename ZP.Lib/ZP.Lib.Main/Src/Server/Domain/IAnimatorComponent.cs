using System;
using UnityEngine;

namespace ZP.Lib
{
    public interface IAnimatorComponent
    {
        void Play(string stateName, int layer = -1, float normalizedTime = float.NegativeInfinity); 
    }
}
