using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Server.Test.Domain;
using ZP.Lib;
using UnityEngine;
using NUnit.Framework;

namespace ZP.Lib.Server.Test.Entity
{
    internal class TestObj : ITestInterface
    {
        public float TestData => testData.Value;

        [PropertyDescription("testData", "a test data")]
        public ZProperty<float> testData = new ZProperty<float>();

        void OnPreUnbind(Transform tran)
        {
            Debug.Log("OnPreUnbind Func");

            Assert.IsTrue(tran.name.CompareTo("Person.testInterface2") == 0);

            GameObject.Destroy(tran.gameObject);
        }

        void ITestInterface.TestFunc()
        {
            Debug.Log("Test Func");
        }
    }
}
