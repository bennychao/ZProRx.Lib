using System;
using UnityEngine;

namespace ZP.Lib
{
    [PropertyValueChangeAnchorClass(".Position", ".Rotation", ".Scale")]
    public class ZTransform3
    {
        public ZProperty<Vector3> Position = new ZProperty<Vector3>();
        public ZProperty<Quaternion> Rotation = new ZProperty<Quaternion>(Quaternion.identity);
        public ZProperty<Vector3> Scale = new ZProperty<Vector3>(Vector3.one);

        public Vector3 AxisScale = Vector3.one;

        static public float Distance(ZTransform3 a, ZTransform3 b)
        {
            return Vector3.Distance(a.Position, b.Position);
        }

        public Vector3 ToAxisPosition3()
        {
            return new Vector3(Position.Value.x * AxisScale.x, Position.Value.y * AxisScale.y, Position.Value.z * AxisScale.z);
        }

        public Vector3 ToPosition3()
        {
            return new Vector3(Position.Value.x , Position.Value.y, Position.Value.z);
        }

        public Quaternion ToRotation()
        {
            return Rotation.Value;
        }


        public void ApplyToLocalTransform(Transform trans)
        {
            trans.localPosition = ToAxisPosition3();
            trans.localRotation = Rotation.Value;
            trans.localScale = Scale.Value;
        }

        public void ApplyToTransform(Transform transform)
        {
            transform.position = ToAxisPosition3();
            transform.rotation = Rotation.Value;
            transform.localScale = Scale.Value;
        }

    }
}
