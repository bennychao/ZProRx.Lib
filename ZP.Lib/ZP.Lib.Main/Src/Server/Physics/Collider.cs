using System;
#if ZP_SERVER
namespace UnityEngine
{
    public abstract class Collider :  ZServerComponent
    {
        public Collider()
        {
        }

        private bool inIsTrigger = false;

        public bool isTrigger
        {
            get => inIsTrigger;
            set => inIsTrigger = value;
        }

        public abstract Bounds bounds { get; }

        public abstract bool CheckCollision(Collider collider);
    }
}

#endif