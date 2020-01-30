using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.Server.Test.Domain;

namespace ZP.Lib.Server.Test.Entity
{
    internal enum DegreeEnum
    {
        UniversityDegree = 1,
        MasterDegree = 2
    }

    internal class Person
    {
        [PropertyDescription("rank", "rankDes")]
        public ZRuntimableProperty<int> rank = new ZRuntimableProperty<int>();

        [PropertyDescription("blood", "bloodDes")]
        public ZProperty<int> blood = new ZProperty<int>();


        public ZRankableProperty<int> blood2 = new ZRankableProperty<int>();

        //Test for private ZProperty op
        private ZProperty<bool> bMale = new ZProperty<bool>();

        public ZProperty<DegreeEnum> degree = new ZProperty<DegreeEnum>();

        [PropertyGroup("Group1")]
        [PropertyAddComponent(typeof(RTTransform3))]
        public ZProperty<Vector3> pos = new ZProperty<Vector3>();

        [PropertyDescription("weapon007", "a power weapon")]
        //[PropertyUIItemRes("Test/Image")]
        public ZProperty<Weapon> weapon = new ZProperty<Weapon>();


        //load the image prefab
        [PropertyDescription("testlist", "a test list")]
       // [PropertyUIItemRes("Test/Image")]
        public ZPropertyList<int> testList = new ZPropertyList<int>();

        //interface property会隔断自动创建过程，这个与ZECS的兼容，还是很有必要的。但是如果是保存呢？
        [PropertyDescription("ITestInterface", "a test interface")]
        public ZProperty<ITestInterface> testInterface = new ZProperty<ITestInterface>();

        [PropertyDescription("ITestInterface2", "a test interface2")]
        [PropertyUIItemRes("Test/TestObject")]
        public ZPropertyInterfaceRef<ITestInterface> testInterface2
        = new ZPropertyInterfaceRef<ITestInterface>(
            (id) =>
            {
                var test = ZPropertyMesh.CreateObject<TestObj>();
                test.testData.Value = 19.9f;
                return test;
            }

            );

        //normal 
        public bool BMale => bMale.Value;

        //public string TestID;

        public Person()
        {
            Debug.Log("Person ");
        }

        public Person(bool bSet)
        {
            Debug.Log("Person bset =" + bSet.ToString());
        }
    }
}
