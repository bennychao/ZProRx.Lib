using System;
using UnityEngine;

namespace ZP.Lib
{

	//public partial class ZBattleTemplate<ActionType, ParamType, T>
	//{
	[PropertyBindSelf]
    [PropertyValueChangeAnchorClass(".Position", ".Direction")]
    public class ZTransform
	{
		public ZProperty<Vector2> Position = new ZProperty<Vector2>();
		public ZProperty<Vector2> Direction = new ZProperty<Vector2>();
        public Vector2 AxisScale = Vector2.one;

        public ZTransform()
        {

        }

        public ZTransform(Vector2 axisScale) 
        {
            this.AxisScale = axisScale;
        }

        static public ZTransform Create(Vector2 pos)
        {
            var ret = ZPropertyMesh.CreateObject<ZTransform>();
            ret.Position.Value = pos;
            ret.Direction.Value = Vector2.up;
            return ret;
        }

        static public ZTransform Create(Vector2 pos, Vector2 dir)
        {
            var ret = ZPropertyMesh.CreateObject<ZTransform>();
            ret.Position.Value = pos;
            ret.Direction.Value = dir;
            return ret;
        }

        static public float Distance(ZTransform a, ZTransform b)
        {
            return Vector2.Distance(a.Position, b.Position);
        }

        //public void ToUnityTransform(Transform trans)
        //{
        //    trans.position = (Vector3)Position.Value;
        //}
        public string ToPosString()
        {
            return Position.Value.ToString();
        }

        public string ToDirString()
        {
            return Direction.Value.ToString();
        }

        public Vector3 ToPosition3()
        {
            return new Vector3(Position.Value.x, 0, Position.Value.y);
        }

        public Vector3 ToPosition3(Vector2 scale)
        {
            return new Vector3(Position.Value.x * scale.x, 0, Position.Value.y * scale.y);
        }

        public Vector3 ToAxisPosition3()
        {
            return new Vector3(Position.Value.x * AxisScale.x, 0, Position.Value.y * AxisScale.y);
        }

        public Vector2 ToAxisPosition()
        {
            return new Vector2(Position.Value.x * AxisScale.x, Position.Value.y * AxisScale.y);
        }

        //public Vector3 ToAxisPosition(Vector2 scale)
        //{
        //    return new Vector3(Position.Value.x * AxisScale.x, 0, Position.Value.y * AxisScale.y);
        //}

        public Quaternion ToRotation()
        {
            var a = Vector2.Angle(Direction.Value, new Vector2(1.0f, 0.0f));

#if !ZP_SERVER
            return Quaternion.Euler(0f, a, 0f);
#else
            return ZTransformEx.EulerToQuat(new Vector3(0.0f, a, 0.0f));
#endif
        }


        public void ApplyPosition(ZTransform pos)
        {
            Position.Value = pos.Position.Value;
        }

        public void ApplyAxisPosition(ZTransform pos)
        {
            Position.Value = pos.ToAxisPosition3();
        }

        public Vector2 ConvertToScaledPosition(ZTransform axisTrans)
        {
            return ConvertToScaledPosition(axisTrans.Position.Value);
        }

        public Vector2 ConvertToScaledPosition(Vector2 pos)
        {
            return new Vector2(Mathf.FloorToInt(pos.x / AxisScale.x),
                Mathf.FloorToInt(pos.y / AxisScale.y));
        }

        public Vector2 ConvertToAxisPosition(ZTransform unityTrans)
        {
            unityTrans.Position.Value = ConvertToAxisPosition(unityTrans.Position.Value);
            return unityTrans.Position.Value;
        }

        public Vector2 ConvertToAxisPosition(Vector2 pos)
        {
            return new Vector2(pos.x * AxisScale.x, pos.y * AxisScale.y);
        }

        public void ApplyToLocalTransform(Transform trans)
        {
            trans.localPosition = ToAxisPosition3();
            Vector3 dir = new Vector3(Direction.Value.x, 0, Direction.Value.y);

#if !ZP_SERVER
            trans.localRotation = Quaternion.LookRotation(dir, Vector3.up); ;//Quaternion.AngleAxis ((float)v.a, Vector3.up);
#else
            trans.localRotation  = ZTransformEx.LookAt(dir, Vector3.up);
#endif
        }

        public void ApplyToTransform(Transform transform)
        {
            transform.position = ToAxisPosition3();
            Vector3 dir = new Vector3(Direction.Value.x, 0, Direction.Value.y);
#if !ZP_SERVER
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up); ;//Quaternion.AngleAxis ((float)v.a, Vector3.up);
#else
            transform.rotation = ZTransformEx.LookAt(dir, Vector3.up);
#endif
        }

          //public static implicit operator ZProperty<T>(T d)  // implicit digit to byte conversion operator
          //{
          //    var newProp = new ZProperty<T>(d);  // implicit conversion
          //    //ZPropertyMgr.BindPropertyAttribute(newProp, ????)
          //    //newProp.AttributeNode = this.attributeNode;

          //    return newProp;
          //}
    }

}

//}

