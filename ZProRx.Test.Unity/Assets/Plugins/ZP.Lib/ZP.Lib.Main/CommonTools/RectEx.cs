using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//version 0.5
namespace ZP.Lib.Common
{
    public static class RectEx
    {

        public static Vector2 GetGap(this Rect rect, Rect other)
        {
            Vector2 centerOffset = rect.center - other.center;
            //centerOffset.

            return new Vector2(Mathf.Abs(centerOffset.x) - (rect.width / 2) - (other.width / 2),
                Mathf.Abs(centerOffset.y) - (rect.height / 2) - (other.height / 2));
        }

        public static bool IsIntersect(this Rect rect, Rect other)
        {
            Vector2 gap = rect.GetGap(other);
            return gap.x <= 0 || gap.y <= 0;
        }

        public static bool Contains(this Rect rect, Rect other)
        {
            return rect.Contains(other.min) && rect.Contains(other.max);
        }


        public static bool IsParallel(this Rect rect, Rect other)
        {
            Vector2 gap = rect.GetGap(other);
            return gap.x <= 0 ^ gap.y <= 0;
        }

        public static void DebugShow(this Rect rect)
        {
            rect.DebugShow(Color.white);
        }

        public static void DebugShow(this Rect rect, Color color)
        {
            //Debug.DrawLine()
            var ps = rect.Points();

            Debug.DrawLine(new Vector3(ps[0].x, 0, ps[0].y), new Vector3(ps[1].x, 0, ps[1].y), color);
            Debug.DrawLine(new Vector3(ps[1].x, 0, ps[1].y), new Vector3(ps[2].x, 0, ps[2].y), color);
            Debug.DrawLine(new Vector3(ps[2].x, 0, ps[2].y), new Vector3(ps[3].x, 0, ps[3].y), color);
            Debug.DrawLine(new Vector3(ps[3].x, 0, ps[3].y), new Vector3(ps[0].x, 0, ps[0].y), color);
        }

        public static List<Vector2> Points(this Rect rect)
        {
            List<Vector2> ret = new List<Vector2>();

            ret.Add(rect.min);
            ret.Add(new Vector2(rect.min.x, rect.min.y + rect.height));
            ret.Add(rect.max);
            ret.Add(new Vector2(rect.min.x + rect.width, rect.min.y));

            return ret;
        }

        public static List<Vector2> IntPoints(this Rect rect)
        {
            List<Vector2> ret = new List<Vector2>();
            var minX = Mathf.CeilToInt(rect.min.x);
            var minY = Mathf.CeilToInt(rect.min.y);

            var maxX = Mathf.FloorToInt(rect.max.x);
            var maxY = Mathf.FloorToInt(rect.max.y);

            for (int j = minY; j <= maxY; j++)
            {
                for (int i = minX; i <= maxX; i++)
                {
                    ret.Add(new Vector2(i, j));
                }
            }

            return ret;
        }

        //-0.5 ~ 0.5 return 1  
        //0 ~ 1 return 2
        public static Vector2 IntSize(this Rect rect)
        {
            var minX = Mathf.CeilToInt(rect.min.x);
            var minY = Mathf.CeilToInt(rect.min.y);

            var maxX = Mathf.FloorToInt(rect.max.x);
            var maxY = Mathf.FloorToInt(rect.max.y);


            return new Vector2(maxX - minX + 1, maxY - minY + 1);
        }


        //other is contained by rect,rect is bigger than other
        public static List<Rect> GetExclusiveInside(this Rect rect, Rect other)
        {
            List<Rect> ret = new List<Rect>();
            var r = new Rect(rect.x, rect.y, other.x - rect.x, rect.height);
            ret.Add(r);
            r = new Rect(other.x, other.max.y, rect.max.x - other.x, (rect.max.y - other.max.y));
            ret.Add(r);
            r = new Rect(other.max.x, rect.y, rect.max.x - other.max.x, rect.height - (rect.max.y - other.max.y));
            ret.Add(r);
            r = new Rect(other.x, rect.y, other.width, other.y - rect.y);
            ret.Add(r);

            return ret;
        }

        //if rect and other is intersect, return the sub rects in rect but not in other.
        public static List<Rect> GetExclusiveIntersect(this Rect rect, Rect other)
        {
            List<Rect> ret = new List<Rect>();

            Vector2 centerOffset = other.center - rect.center;
            float offsetXBorder = ((rect.width - other.width) / 2 + Mathf.Abs(centerOffset.x)) * (centerOffset.x > 0 ? 1 : -1);
            float offsetYBorder = ((rect.width - other.width) / 2 + Mathf.Abs(centerOffset.y)) * (centerOffset.y > 0 ? 1 : -1);

            if (centerOffset.x >= 0 && centerOffset.y >= 0)
            {
                var r1 = new Rect(rect.x, rect.y, offsetXBorder, rect.height);

                var r2 = new Rect(rect.x + offsetXBorder, rect.y, rect.width - offsetXBorder, offsetYBorder);

                ret.Add(r1);
                ret.Add(r2);
            }
            else if (centerOffset.x >= 0 && centerOffset.y <= 0)
            {
                var r1 = new Rect(rect.x, rect.y, Mathf.Abs(offsetXBorder), rect.height);

                var r2 = new Rect(rect.x + Mathf.Abs(offsetXBorder), rect.y + rect.height - Mathf.Abs(offsetYBorder), rect.width - Mathf.Abs(offsetXBorder), Mathf.Abs(offsetYBorder));
                ret.Add(r1);
                ret.Add(r2);
            }
            else if (centerOffset.x <= 0 && centerOffset.y >= 0)
            {
                var r1 = new Rect(rect.x, rect.y, rect.width - Mathf.Abs(offsetXBorder), Mathf.Abs(offsetYBorder));

                var r2 = new Rect(rect.x + rect.width - Mathf.Abs(offsetXBorder), rect.y, Mathf.Abs(offsetXBorder), rect.height);
                ret.Add(r1);
                ret.Add(r2);
            }
            else if (centerOffset.x <= 0 && centerOffset.y <= 0)
            {
                var r1 = new Rect(rect.x, rect.y + rect.height - Mathf.Abs(offsetYBorder), rect.width, Mathf.Abs(offsetYBorder));

                var r2 = new Rect(rect.x + rect.width - Mathf.Abs(offsetXBorder), rect.y, Mathf.Abs(offsetXBorder), rect.height - Mathf.Abs(offsetYBorder));
                ret.Add(r1);
                ret.Add(r2);
            }
            return ret;
        }

        public static int GetIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 contractPoint)
        {
            contractPoint = new Vector3(0, 0);

            if (Mathf.Abs(b.z - a.z) + Mathf.Abs(b.x - a.x) + Mathf.Abs(d.z - c.z)
                    + Mathf.Abs(d.x - c.x) < 0.00001)
            {
                if (Mathf.Abs((c.x - a.x) + (c.z - a.z)) < 0.00001)
                {
                    //Debug.Log("ABCD是同一个点！");
                }
                else
                {
                    //Debug.Log("AB是一个点，CD是一个点，且AC不同！");
                }
                return 0;
            }

            if (Mathf.Abs(b.z - a.z) + Mathf.Abs(b.x - a.x) < 0.00001)
            {
                if (Mathf.Abs((a.x - d.x) * (c.z - d.z) - (a.z - d.z) * (c.x - d.x)) < 0.00001)
                {
                    //Debug.Log("A、B是一个点，且在CD线段上！");
                }
                else
                {
                    //Debug.Log("A、B是一个点，且不在CD线段上！");
                }
                return 0;
            }
            if (Mathf.Abs(d.z - c.z) + Mathf.Abs(d.x - c.x) < 0.00001)
            {
                if (Mathf.Abs((d.x - b.x) * (a.z - b.z) - (d.z - b.z) * (a.x - b.x)) < 0.00001)
                {
                    //Debug.Log("C、D是一个点，且在AB线段上！");
                }
                else
                {
                    //Debug.Log("C、D是一个点，且不在AB线段上！");
                }
                return 0;
            }

            if (Mathf.Abs((b.z - a.z) * (c.x - d.x) - (b.x - a.x) * (c.z - d.z)) < 0.00001)
            {
                //Debug.Log("线段平行，无交点！");
                return 0;
            }

            contractPoint.x = ((b.x - a.x) * (c.x - d.x) * (c.z - a.z) -
                    c.x * (b.x - a.x) * (c.z - d.z) + a.x * (b.z - a.z) * (c.x - d.x)) /
                    ((b.z - a.z) * (c.x - d.x) - (b.x - a.x) * (c.z - d.z));
            contractPoint.z = ((b.z - a.z) * (c.z - d.z) * (c.x - a.x) - c.z
                    * (b.z - a.z) * (c.x - d.x) + a.z * (b.x - a.x) * (c.z - d.z))
                    / ((b.x - a.x) * (c.z - d.z) - (b.z - a.z) * (c.x - d.x));

            if ((contractPoint.x - a.x) * (contractPoint.x - b.x) <= 0
                    && (contractPoint.x - c.x) * (contractPoint.x - d.x) <= 0
                    && (contractPoint.z - a.z) * (contractPoint.z - b.z) <= 0
                    && (contractPoint.z - c.z) * (contractPoint.z - d.z) <= 0)
            {

                //Debug.Log("线段相交于点(" + contractPoint.x + "," + contractPoint.z + ")！");
                return 1; // '相交  
            }
            else
            {
                //Debug.Log("线段相交于虚交点(" + contractPoint.x + "," + contractPoint.z + ")！");
                return -1; // '相交但不在线段上  
            }
        }
    }


}

