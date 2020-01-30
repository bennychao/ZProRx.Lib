using System;
//using System.Numerics;
using UnityEngine;

namespace ZP.Lib
{

    enum RotSeq { zyx, zyz, zxy, zxz, yxz, yxy, yzx, yzy, xyz, xyx, xzy, xzx };

    public static class Quat2Euler
    {

        //    case zxy:
        //threeaxisrot(-2 * (q.x * q.y - q.w * q.z),
        // q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
        // 2 * (q.y * q.z + q.w * q.x),
        //-2 * (q.x * q.z - q.w * q.y),
        //q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
        //res);

        //    case zyx:
        //threeaxisrot(2 * (q.x * q.y + q.w * q.z),
        // q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
        //-2 * (q.x * q.z - q.w * q.y),
        //2 * (q.y * q.z + q.w * q.x),
        //q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
        //res);

        //    case yxz: use this inverse???
        //threeaxisrot( 2*(q.x* q.z + q.w* q.y),
                    // q.w* q.w - q.x* q.x - q.y* q.y + q.z* q.z,
                    //-2*(q.y* q.z - q.w* q.x),
                     //2*(q.x* q.y + q.w* q.z),
                     //q.w* q.w - q.x* q.x + q.y* q.y - q.z* q.z,
                     //res);

        static void twoaxisrot(double r11, double r12, double r21, double r31, double r32, double[] res)
        {
            res[0] = Math.Atan2(r11, r12);
            res[1] = Math.Acos(r21);
            res[2] = Math.Atan2(r31, r32);
        }

        static Vector3 threeaxisrot(double r11, double r12, double r21, double r31, double r32)
        {
            return new Vector3(     // * 180.0f / Mathf.PI
            (float)Math.Asin(r21),  // * 180.0f / Mathf.PI
                (float)Math.Atan2(r11, r12), (float)Math.Atan2(r31, r32)) * 180.0f / Mathf.PI;
        }

        static public Vector3 QuaternionToEuler(Quaternion q) // Z-Y-X Euler angles
        {
            Vector3 euler = Vector3.zero;
            //const double Epsilon = 0.0009765625f;
            //const double Threshold = 0.5f - Epsilon;

            //double TEST = q.w * q.y - q.x * q.z;

            ////if (TEST < -Threshold || TEST > Threshold) // 奇异姿态,俯仰角为±90°
            ////{
            ////    int sign = Math.Sign(TEST);

            ////    euler.z =(float)( -2.0 * sign * (double)Math.Atan2(q.x, q.w)); // yaw

            ////    euler.y = (float)(sign * (Math.PI / 2.0)); // pitch

            ////    euler.x = 0; // roll

            ////}
            ////else
            {

                euler = threeaxisrot(2 * (q.x * q.z + q.w * q.y),
                     q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                    -2 * (q.y * q.z - q.w * q.x),
                     2 * (q.x * q.y + q.w * q.z),
                     q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);
                //euler.x = (float)Math.Atan2(2.0f * (q.y * q.z + q.w * q.x), q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);
                //euler.y = (float)Math.Asin(-2 * (q.x * q.z - q.w * q.y));
                //euler.z = (float)Math.Atan2(2 * (q.x * q.y + q.w * q.z), q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);
            }


            return euler;
        }
    }
}
