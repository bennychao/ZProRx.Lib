using System;
#if ZP_SERVER
using ZP.Lib.Common;
using ZP.Lib;

namespace UnityEngine
{
    public class BoxCollider : Collider//, IBindable
    {
        public BoxCollider()
        {
        }

        private Vector3 inSize;

        public Vector3 size
        {
            get => inSize;
            set => inSize = value;
        }

        public override Bounds bounds => this.GetRawBounds();

        //public void BindData(BindComponentParam param)
        //{
        //    if (string.CompareOrdinal( param.Name.Value, "size") == 0)
        //    {

        //    }
        //}

        public override bool CheckCollision(Collider collider)
        {
            if (this == collider)
                return false;

            return bounds.Intersects(collider.bounds) || collider.bounds.Intersects(bounds);
        }
    }
}

#endif