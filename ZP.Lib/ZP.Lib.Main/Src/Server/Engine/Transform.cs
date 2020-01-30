using System;
#if ZP_SERVER

using ZP.Lib;

namespace UnityEngine
{
    public class Transform : ZServerComponent
    {

        public Transform()
        {
            localScale = Vector3.one;
        }

        
        new public bool enabled
        {
            // set => inIsActive = value;
            get => inIsActive;
        }


        //base
        public Transform parent
        {
            get;
            set;
        }

        //base
        public Vector3 localPosition
        {
            get;
            set;
        }

        public Vector3 position
        {
            get => ZTransformEx.GetGlobalPosition(this);
            set =>
               localPosition = ZTransformEx.GlobalToLocalPosition(this, value);
        }

        public Vector3 localEulerAngles
        {
            get => ZTransformEx.ToEuler(localRotation);
            set => localRotation = ZTransformEx.EulerToQuat(value);
        }

        public Vector3 eulerAngles
        {
            get => ZTransformEx.ToEuler(rotation);
            set => rotation = ZTransformEx.EulerToQuat(value);
        }

        public Vector3 right
        {
            get => -ZTransformEx.Left(this);
            //set;
        }

        public Vector3 up
        {
            get => ZTransformEx.Up(this);
            //set;
        }

        public Vector3 forward
        {
            get => ZTransformEx.Forward(this);
            //set;
        }

        public Quaternion rotation
        {
            get => ZTransformEx.GetGlobalRotation(transform);
            set => localRotation = ZTransformEx.GlobalToLocalRotation(this, value);
        }

        //base
        public Quaternion localRotation
        {
            get;
            set;
        }

        //base
        public Vector3 localScale
        {
            get;
            set;
        }

        public Vector3 lossyScale
        {
            get => GetGlobalScale(this);
        }

        public int childCount
        {
            //set;
            get => GetChildCount();
        }

        public Matrix4x4 localToWorldMatrix
        {
            get =>  ZTransformEx.ToUnityMatrix4x4( ZTransformEx.LocalToGlobalMatrix(transform));
        }

        public Matrix4x4 worldToLocalMatrix
        {
            get => ZTransformEx.ToUnityMatrix4x4(ZTransformEx.GlobalToLocalMatrix(transform));
        }

        public void SetSiblingIndex(int index)
        {
            //TODO
        }

        //internal override void LoadParams(BindComponent bindComponent)
        //{

        //}

        public int GetChildCount()
        {
            return gameObject.Subs.Count;
        }



        public Transform GetChild(int index)
        {
            return gameObject.Subs[index]?.transform;
        }

        //not support transform.Find("magazine/ammo");
        public Transform Find(string name)
        {
            return gameObject.Subs.Find(obj=> 
            string.Compare(obj.name, name, StringComparison.Ordinal) == 0)?.transform;
        }

        static Vector3 GetGlobalScale(Transform transform)
        {
            Vector3 ret = transform.localScale;
            var curparent = transform.parent;
            while (curparent != null)
            {
                ret =new Vector3(  ret.x * curparent.localScale.x, 
                    ret.y * curparent.localScale.y,
                     ret.z * curparent.localScale.z);
                curparent = curparent.parent;
            }

            return ret;
        }

        public void LookAt(Transform target, Vector3 worldUp)
        {
            this.rotation = ZTransformEx.LookAt(target.position - position, worldUp);
        }

        public void LookAt(Transform target)
        {
            this.rotation = ZTransformEx.LookAt(target.position - position, Vector3.up);
        }

        public void LookAt(Vector3 dir, Vector3 worldUp)
        {
            this.rotation = ZTransformEx.LookAt(dir, worldUp);
        }

        public void LookAt(Vector3 dir)
        {
            this.rotation =  ZTransformEx.LookAt(dir, Vector3.up);
        }

        public void DetachChildren()
        {
            this.gameObject.RemoveAll();
        }
    }
}
#endif
