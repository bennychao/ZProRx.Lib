using System;
using UnityEngine;

namespace ZP.Lib
{
    public interface IZRigidbodyComponent
    {
        void AddForce(Vector2 force);
        void AddForceToDir(Vector2 dir);

        void AddForce(Vector3 force);

        void Reset();
    }
}
