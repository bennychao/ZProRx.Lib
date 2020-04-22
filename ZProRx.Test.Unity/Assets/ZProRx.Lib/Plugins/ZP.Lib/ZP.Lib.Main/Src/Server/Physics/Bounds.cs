using System;
#if ZP_SERVER
namespace UnityEngine
{
    public class Bounds
    {
        private Vector3 m_Center;

        private Vector3 m_Extents;

        public Vector3 center
        {
            get => m_Center;
            set => m_Center= value;
        }

        public Vector3 size
        {
            get => m_Extents * 2;
            set => m_Extents = value / 2;
        }

        public Vector3 extents
        {
            get => m_Extents;
            set => m_Extents = value;
        }

        public Vector3 min
        {
            get => m_Center - m_Extents;
            //set => m_Center.;
        }

        public Vector3 max
        {
            get => m_Center + m_Extents;
            //set;
        }

        public Bounds(Vector3 center, Vector3 size)
        {
            m_Center = center;
            m_Extents = size / 2;
        }
            //size is same
        public bool Equals(Bounds other)
        {
            return size == other.size;
        }


        public bool Intersects(Bounds bounds)
        {
            return Contains(bounds.max) || Contains(bounds.min) || bounds.Contains(max) || bounds.Contains(min);
        }


        public bool Contains(Vector3 point)
        {
            //float a = Vector3.Angle(max - point, min - point);
            //return a > 90;
            var ptom = point - max;
            var pton = point - min;

            return pton.x > 0 && pton.y > 0 && pton.z > 0 && ptom.x < 0 && ptom.y < 0 && ptom.z < 0;
        }

        public bool Contains(Bounds bounds)
        {
            return Contains(bounds.max) && Contains(bounds.min);
        }

        //public override int GetHashCode();

        //public override bool Equals(object other);

        //public static bool operator ==(Bounds_client lhs, Bounds_client rhs);

        //public static bool operator !=(Bounds_client lhs, Bounds_client rhs);

        //public void SetMinMax(Vector3 min, Vector3 max);

        //public void Encapsulate(Vector3 point);

        //public void Encapsulate(Bounds_client bounds);

        //public void Expand(float amount);

        //public void Expand(Vector3 amount);


        //public bool IntersectRay(Ray ray);

        //public bool IntersectRay(Ray ray, out float distance);

        //public override string ToString();

        //public string ToString(string format);

        //[FreeFunction("BoundsScripting::SqrDistance", HasExplicitThis = true, IsThreadSafe = true)]
        //public float SqrDistance(Vector3 point);

        //[FreeFunction("IntersectRayAABB", IsThreadSafe = true)]
        //private static bool IntersectRayAABB(Ray ray, Bounds_client bounds, out float dist);

        //[FreeFunction("BoundsScripting::ClosestPoint", HasExplicitThis = true, IsThreadSafe = true)]
        //public Vector3 ClosestPoint(Vector3 point);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private static extern bool Contains_Injected(ref Bounds_client _unity_self, ref Vector3 point);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private static extern float SqrDistance_Injected(ref Bounds_client _unity_self, ref Vector3 point);

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //private static extern bool IntersectRayAABB_Injected(ref Ray ray, ref Bounds_client bounds, out float dist);


    }
}
#endif