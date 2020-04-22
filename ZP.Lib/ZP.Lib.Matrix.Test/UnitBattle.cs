using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix.Test
{
    public class UnitBattle
    {
        [SetUp]
        public void Setup()
        {
            ServerPath.WorkPath = "../../..";
            ServerPath.AppName = "MatrixTest";
        }

        [Test]
        public void TestLocalBattle()
        {

        }
    }
}
