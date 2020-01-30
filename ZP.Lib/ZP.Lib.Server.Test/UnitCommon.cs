using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Common;

namespace ZP.Lib.Server.Test
{
    // E1 and E2 must be continuous int/uint
    enum E1
    {
        V1, V2, V3, VMax
    }

    enum E2
    {
        VE1 = E1.VMax + 1, VE2, VE3, VEMax
    }

    public class TestCommon
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestMultiEnum()
        {

            MultiEnum<E1, E2> multiEnum = new MultiEnum<E1, E2>();
            var str = multiEnum.ToString();

            multiEnum = E2.VE2;
            E2 e2 = E2.VE2;

            Assert.IsTrue(multiEnum == E2.VE2);

            Assert.IsTrue(multiEnum != E1.V1);
            Assert.IsTrue(multiEnum == e2);

            multiEnum.Parse("V1");
            Assert.IsTrue(multiEnum == E1.V1);

            Assert.IsTrue(E1.V2 != multiEnum);

            //not support 
            //switch (multiEnum)
            //{
            //    case E2.VE2:
            //        return;

            //    case E1.V2:
            //        return;

            //    default:
            //        return;
            //        //return
            //}

            switch ((int)multiEnum)
            {
                case (int)E2.VE2:
                    Assert.IsTrue(false);
                    return;

                case (int)E1.V1:
                    Assert.IsTrue(true);
                    return;

                default:
                    return;
                    //return
            }

        }
    }
}
