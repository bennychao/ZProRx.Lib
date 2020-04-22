using System;
using System.Collections.Generic;
using System.Reflection;
using ZP.Lib;

#if ZP_SERVER
namespace UnityEngine
{
    public static class Physics
    {

        static List<Collision> enterCollsion = new List<Collision>();

        //[TODO]
        public static List<Transform> OverlapBox(Vector3 pos, Vector3 range)
        {
            List<Transform> ret = new List<Transform>();

            return ret;
        }


        internal static void FixedUpdate(ZServerScene scene)
        {
            var cs = scene.GetAllComponents<Collider>();
            foreach (var c in cs)
            {
                if (c.enabled == false)
                    continue;

                var other =cs.Find((Collider obj) => obj != c &&  obj.enabled && obj.CheckCollision(c));
                if (other != null)
                {
                    if (IsInCollision(c, other))
                        continue;
                    else
                    {
                        AddCollision(c, other);
                    }
                }
                else
                {
                    DelCollision(c);
                }
            }

            //check leave collision

        }


        internal static bool IsInCollision(Collider collider, Collider other)
        {
            return enterCollsion.Find((Collision obj) => obj.collider == collider 
            && obj.other == other) != null;
        }

        internal static bool IsInCollision(GameObject obj)
        {
            return enterCollsion.Find((Collision col) => col.gameObject == obj
            || col.other.gameObject == obj) != null;
        }

        internal static void AddCollision(Collider collider, Collider other)
        {
            Collision collision = new Collision();
            collision.collider = collider;
            collision.other = other;

            enterCollsion.Add(collision);

            if (collider.isTrigger || other.isTrigger)
                InvokeTriggerEnterMethod(other.gameObject, collider);
            else
                InvokeCollisionEnterMethod(other.gameObject, collision);

            //to other
            //Collision otherCollision = new Collision();
            //otherCollision.collider = collider;
            //otherCollision.other = other;

            //enterCollsion.Add(otherCollision);

            //if (collider.isTrigger || other.isTrigger)
            //    InvokeTriggerEnterMethod(other.gameObject, collider);
            //else
            //    InvokeCollisionEnterMethod(other.gameObject, otherCollision);
        }

        internal static void DelCollision(Collider collider, Collider other)
        {
            enterCollsion.RemoveAll((Collision obj) =>
            {
                return (obj.collider == collider && obj.other == other) ||
                (obj.collider == other && obj.other == collider);
            });

            if (collider.isTrigger || other.isTrigger)
            {
                InvokeTriggerExitMethod(collider.gameObject, other);
                InvokeTriggerExitMethod(other.gameObject, collider);
            }
            else { 
                Collision collision = new Collision();
                collision.collider = collider;
                collision.other = other;
                
                InvokeCollisionExitMethod(other.gameObject, collision);

                collision.collider = other;
                collision.other = collider;
                
                InvokeCollisionExitMethod(collider.gameObject, collision);
            }
        }

        internal static void DelCollision(Collider collider)
        {
            var list = enterCollsion.FindAll((Collision obj) =>
            {
                return (obj.collider == collider) ||
                (obj.other == collider);
            });

            foreach (var c in list)
            {
                enterCollsion.Remove(c);
                if (c.collider.isTrigger || c.other.isTrigger)
                    InvokeTriggerExitMethod(c.collider.gameObject, c.collider);
                else
                    InvokeCollisionExitMethod(c.collider.gameObject, c);
            }

            //InvokeCollisionExitMethod(collider.gameObject);

            //InvokeCollisionExitMethod(.gameObject);
        }



        static private void InvokeCollisionEnterMethod(GameObject obj, Collision collision)
        {
            var cs = obj.GetComponents<MonoBehaviour>();
            foreach (var c in cs)
            {
                Type type = c.GetType();
                // BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                MethodInfo method = type.GetMethod("OnCollisionEnter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
                object[] parameters = { collision };

                if (method != null)
                    method.Invoke(c, parameters);                           // 调用方法，参数为空
            }


        }

        static private void InvokeCollisionExitMethod(GameObject obj, Collision collision)
        {
            var cs = obj.GetComponents<MonoBehaviour>();
            foreach (var c in cs)
            {
                Type type = c.GetType();
                // BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                MethodInfo method = type.GetMethod("OnCollisionExit", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
                object[] parameters = { collision };

                if (method != null)
                    method.Invoke(c, parameters);                           // 调用方法，参数为空
            }

        }
        static private void InvokeTriggerEnterMethod(GameObject obj, Collider other)
        {
            var cs = obj.GetComponents<MonoBehaviour>();
            foreach (var c in cs)
            {
                Type type = c.GetType();
                // BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                MethodInfo method = type.GetMethod("OnTriggerEnter", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
                object[] parameters = { other }; 

                if (method != null)
                    method.Invoke(c, parameters);                           // 调用方法，参数为空
            }

        }

        static private void InvokeTriggerExitMethod(GameObject obj, Collider other)
        {
            var cs = obj.GetComponents<MonoBehaviour>();
            foreach (var c in cs)
            {

                Type type = c.GetType();
                // BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                MethodInfo method = type.GetMethod("OnTriggerExit", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
                object[] parameters = { other }; 

                if (method != null)
                    method.Invoke(c, parameters);                           // 调用方法，参数为空
            }
        }
    }
}

#endif