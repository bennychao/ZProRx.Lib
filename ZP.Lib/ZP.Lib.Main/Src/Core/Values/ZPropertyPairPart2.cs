using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    public class ZPropertyPairPart2<T1, T2>
    {

        //[PropertySecondProperty("Part1")]
        public ZPartProperty<T1> Part1 = new ZPartProperty<T1>();
        //[PropertySecondProperty("Part2")]
        public ZPartProperty<T2> Part2 = new ZPartProperty<T2>();

        public ZPropertyPairPart2()
        {

        }

        public ZPropertyPairPart2(string part1Name, T1 value1, string part2Name, T2 value2)
        {
            // Part1.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>().SecondName = part1Name;
            Part1.PartName = part1Name;
            Part1.Value = value1;

            //Part2.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>().SecondName = part2Name;
            Part2.PartName = part2Name;
            Part2.Value = value2;
        }

        public void SetPart1(string part1Name, T1 value1)
        {
            //var attr = Part1.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>();
            //attr.SecondName = part1Name;
            //Debug.Log("Second name is " + attr.SecondName);
            Part1.PartName = part1Name;
            Part1.Value = value1;
        }

        public void SetPart2(string part2Name, T2 value2)
        {
            //Part2.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>().SecondName = part2Name;
            //Debug.Log("Second name is " + Part2.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>().SecondName);
            Part2.PartName = part2Name;
            Part2.Value = value2;
        }

        static public ZPropertyPairPart2<T1, T2> Cache
        {
            get
            {
                return ZPropertyCache<ZPropertyPairPart2<T1, T2>>.Cache;
            }

        }

        static public ZPropertyPairPart2<T1, T2> Create(string part1Name, T1 value1, string part2Name, T2 value2)
        {
            var ret = ZPropertyMesh.CreateObject<ZPropertyPairPart2<T1, T2>>();

            ret.SetPart1(part1Name, value1);
            ret.SetPart2(part2Name, value2);

            return ret;
        }

        static public ZPropertyPairPart2<T1, T2> Create(T1 value1, T2 value2)
        {
            var ret = ZPropertyMesh.CreateObject<ZPropertyPairPart2<T1, T2>>();

            ret.Part1.Value = value1;
            ret.Part2.Value = value2;

            return ret;
        }
    }
}

