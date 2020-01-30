using System;
#if ZP_SERVER
using UnityEngine;

namespace ZP.Lib
{
    public static class ZTransformEx
    {

        internal static System.Numerics.Vector3 ToServerVector3(Vector3 vector3) {
            return new System.Numerics.Vector3(vector3.x, vector3.y, vector3.z);
        }

        internal static Vector3 ToUnityVector3(System.Numerics.Vector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }


        internal static Matrix4x4 ToUnityMatrix4x4(System.Numerics.Matrix4x4 mat)
        {
            var retm = new Matrix4x4();

            retm.m00 = mat.M11;
            retm.m01 = mat.M12;
            retm.m02 = mat.M13;
            retm.m03 = mat.M14;

            retm.m10 = mat.M21;
            retm.m11 = mat.M22;
            retm.m12 = mat.M23;
            retm.m13 = mat.M24;

            retm.m20 = mat.M31;
            retm.m21 = mat.M32;
            retm.m22 = mat.M33;
            retm.m23 = mat.M34;

            retm.m30 = mat.M41;
            retm.m31 = mat.M42;
            retm.m32 = mat.M43;
            retm.m33 = mat.M44;
            return retm;
        }

        internal static Quaternion ToUnityQuaternion(System.Numerics.Quaternion q)
        {
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }

        internal static System.Numerics.Quaternion ToServerQuaternion(Quaternion q)
        {
            return new System.Numerics.Quaternion(q.x, q.y, q.z, q.w);
        }

        internal static System.Numerics.Matrix4x4 LocalToGlobalMatrix(Transform transform)
        {
            var curparent = transform;

            System.Numerics.Matrix4x4 m = System.Numerics.Matrix4x4.Identity;
            while (curparent != null)
            {
                //ret += curparent.localPosition;

                var cm = LocalTRS(curparent);
                m = System.Numerics.Matrix4x4.Multiply(m, cm);
                //var tmp = System.Numerics.Vector3.Transform(ToServerVector3(ret), m);

                //ret = ToUnityVector3(tmp);
                curparent = curparent.parent;
            }
            return m;
        }

        internal static System.Numerics.Matrix4x4 LocalToGlobalRMatrix(Transform transform)
        {
            var curparent = transform;

            System.Numerics.Matrix4x4 m = System.Numerics.Matrix4x4.Identity;
            while (curparent != null)
            {
                //ret += curparent.localPosition;

                var cm = LocalR(curparent);
                m = System.Numerics.Matrix4x4.Multiply(m, cm);
                //var tmp = System.Numerics.Vector3.Transform(ToServerVector3(ret), m);

                //ret = ToUnityVector3(tmp);
                curparent = curparent.parent;
            }
            return m;
        }

        internal static System.Numerics.Matrix4x4 GlobalToLocalMatrix(Transform transform)
        {
            var curparent = transform.parent;

            System.Numerics.Matrix4x4 m = System.Numerics.Matrix4x4.Identity;
            while (curparent != null)
            {
                //ret += curparent.localPosition;

                var cm = LocalTRS(curparent);
                m = System.Numerics.Matrix4x4.Multiply(m, cm);
                //var tmp = System.Numerics.Vector3.Transform(ToServerVector3(ret), m);

                //ret = ToUnityVector3(tmp);
                curparent = curparent.parent;
            }

            System.Numerics.Matrix4x4 im;
            System.Numerics.Matrix4x4.Invert(m, out im);

            return im;
        }

        internal static System.Numerics.Matrix4x4 ServerTRS(Vector3 pos, Quaternion quaternion, Vector3 scale)
        {
            System.Numerics.Matrix4x4 t = System.Numerics.Matrix4x4.CreateTranslation(ToServerVector3(pos));

            System.Numerics.Matrix4x4 r = System.Numerics.Matrix4x4.CreateFromQuaternion(ToServerQuaternion(quaternion));

            System.Numerics.Matrix4x4 s = System.Numerics.Matrix4x4.CreateScale(ToServerVector3(scale));

            return System.Numerics.Matrix4x4.Multiply (System.Numerics.Matrix4x4.Multiply(s, r), t); //must this order
        }

        internal static System.Numerics.Matrix4x4 ServerR(Quaternion quaternion)
        {

            System.Numerics.Matrix4x4 r = System.Numerics.Matrix4x4.CreateFromQuaternion(ToServerQuaternion(quaternion));

            return r; //must this order
        }

        internal static System.Numerics.Matrix4x4 LocalTRS(Transform transform)
        {
            return ServerTRS(transform.localPosition, transform.localRotation, transform.localScale);
        }

        internal static System.Numerics.Matrix4x4 LocalR(Transform transform)
        {
            return ServerR(transform.localRotation);
        }

        //public method -----------------------------------
        public static Quaternion AngleAxisEx(float a, Vector3 upAxis) {
            var q = System.Numerics.Quaternion.CreateFromAxisAngle(ToServerVector3(upAxis), a * Mathf.PI/ 180.0f);

            return ToUnityQuaternion(q);
        }

        public static Vector3 GetGlobalPosition(Transform transform)
        {
            Vector3 ret = transform.localPosition;

            var curparent = transform.parent;
            while (curparent != null)
            {
                //ret += curparent.localPosition;

                var m = LocalTRS(curparent);

                var tmp = System.Numerics.Vector3.Transform(ToServerVector3(ret), m);

                ret = ToUnityVector3(tmp);
                curparent = curparent.parent;
            }

            return ret;
        }

        public static Vector3 Left(Transform transform)
        {
            var m = LocalToGlobalRMatrix(transform);
            return ToUnityVector3( System.Numerics.Vector3.Transform(ToServerVector3( Vector3.left), m));
        }

        public static Vector3 Up(Transform transform)
        {
            var m = LocalToGlobalRMatrix(transform);
            return ToUnityVector3(System.Numerics.Vector3.Transform(ToServerVector3(Vector3.up), m));
        }

        public static Vector3 Forward(Transform transform)
        {
            var m = LocalToGlobalRMatrix(transform);
            return ToUnityVector3(System.Numerics.Vector3.Transform(ToServerVector3(Vector3.forward), m));
        }

        public static Vector3 GlobalToLocalPosition(Transform transform, Vector3 gPos)
        {
            Vector3 ret = transform.localPosition;

            var im = GlobalToLocalMatrix(transform);

            var tmp = System.Numerics.Vector3.Transform(ToServerVector3(gPos), im);

            return ToUnityVector3(tmp);
        }

        public static Quaternion GlobalToLocalRotation(Transform transform, Quaternion gQuat)
        {
            Vector3 ret = transform.localPosition;

            var im = GlobalToLocalMatrix(transform);

            var tmp = System.Numerics.Quaternion.Multiply(ToServerQuaternion( gQuat), 
                System.Numerics.Quaternion.CreateFromRotationMatrix(im));

            return ToUnityQuaternion(tmp);
        }

        public static Quaternion GetGlobalRotation(Transform transform)
        {
            var q1 = ToServerQuaternion(transform.localRotation);

            var curparent = transform.parent;
            while (curparent != null)
            {
                // ret += parent.localRotation;

                var q2 = ToServerQuaternion(curparent.localRotation);
                var q3 = System.Numerics.Quaternion.Multiply(q2, q1); //must this order
                q1 = q3;
                curparent = curparent.parent;
            }

            return ToUnityQuaternion(q1);
        }

        public static Vector3 ToEuler(Quaternion q)
        {
            return Quat2Euler.QuaternionToEuler(q);
        }

        public static Vector3 AngleNormalize(this Vector3 a)
        {
            return new Vector3((a.x + 3600) % 360, (a.y + 3600) % 360, (a.z + 3600) % 360);
        }

        //error
        // public static Vector3 ToEuler(Quaternion q)
        // {
        //     Vector3 ret = -Vector3.left;

        //     var m = System.Numerics.Matrix4x4.CreateFromQuaternion(ToServerQuaternion(q));

        //     var vec = ToUnityVector3(System.Numerics.Vector3.Transform(ToServerVector3(ret), m));

        //     var faceX = new Vector3(0, vec.y, vec.z);
        //     var faceY = new Vector3(vec.x, 0, vec.z);
        //     var faceZ = new Vector3(vec.x, vec.y, 0);

        //     float ax = Vector3.Angle(faceX, ret);
        //     float ay = Vector3.Angle(faceY, ret);
        //     float az = Vector3.Angle(faceZ, ret);

        //     return new Vector3(ax, ay, az);
        // }

        public static Quaternion EulerToQuat(Vector3 euler)
        {
            //CreateFromYawPitchRoll angle param in radians
            var q =  System.Numerics.Quaternion.CreateFromYawPitchRoll(euler.y * Mathf.PI / 180.0f, euler.x * Mathf.PI / 180.0f, euler.z * Mathf.PI / 180.0f);
            return ToUnityQuaternion(q);
        }

        public static Matrix4x4 TRS(Vector3 pos, Quaternion quaternion, Vector3 scale)
        {
            return ToUnityMatrix4x4(ServerTRS(pos, quaternion, scale));
        }

        public static Quaternion LookAt(Vector3 forward, Vector3 upAxis)
        {
            //Debug.Log("LookAt");
            var up = ToServerVector3(upAxis);
            var fwd = ToServerVector3(forward);
           // float a = 0;

            var m = System.Numerics.Matrix4x4.CreateLookAt(fwd, System.Numerics.Vector3.Zero, up);

            //m = System.Numerics.Matrix4x4.Negate(m);
            //var mm = Matrix4x4.LookAt(Vector3.zero, forward, upAxis);

            var q = System.Numerics.Quaternion.CreateFromRotationMatrix(m);

            // [TODO]
            q = System.Numerics.Quaternion.Inverse(q);
            return ToUnityQuaternion(q );
        }

        

    }
}
#endif