using System.Collections;
using System.Collections.Generic;


namespace ZP.Lib
{
    public class ZPropertyPairPart3<T1, T2, T3>
    {
       // [PropertySecondProperty("Part1")]
        public ZPartProperty<T1> Part1 = new ZPartProperty<T1>();
       // [PropertySecondProperty("Part2")]
        public ZPartProperty<T2> Part2 = new ZPartProperty<T2>();
       // [PropertySecondProperty("Part3")]
        public ZPartProperty<T3> Part3 = new ZPartProperty<T3>();

        public ZPropertyPairPart3()
        {

        }

        public ZPropertyPairPart3(string part1Name, T1 value1, string part2Name, T2 value2, string part3Name, T3 value3)
        {
            //Part1.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>().SecondName = part1Name;
            Part1.Value = value1;
            Part1.PartName = part1Name;

            //Part2.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>().SecondName = part2Name;
            Part2.Value = value2;
            Part2.PartName = part2Name;

            //Part3.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>().SecondName = part3Name;
            Part3.Value = value3;
            Part3.PartName = part3Name;
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

        public void SetPart3(string part3Name, T3 value3)
        {
            //Part3.AttributeNode.GetAttribute<PropertySecondPropertyAttribute>().SecondName = part3Name;
            Part3.PartName = part3Name;
            Part3.Value = value3;
        }

        static public ZPropertyPairPart3<T1, T2, T3> Cache
        {
            get
            {
                return ZPropertyCache<ZPropertyPairPart3<T1, T2, T3>>.Cache;
            }

        }
    }
}

