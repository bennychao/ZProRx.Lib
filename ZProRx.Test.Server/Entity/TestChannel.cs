using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UnityEngine;
using ZP.Lib;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Matrix.Test.Entity;
using ZP.Lib.Net;

namespace ZP.Lib.Matrix.Test.Entity
{


    [ChannelBoot(ChannelBootFlagEnum.RoomSuite | ChannelBootFlagEnum.Normal)]
    public class TestChannel : BaseChannel
    {
        IRoomServer roomServer = null;
        ILogger<TestChannel> logger = null;


        // Ioc roomServer
        public TestChannel(IRoomServer roomServer, ILogger<TestChannel> logger) : base()
        {
            this.roomServer = roomServer;
            Assert.IsTrue(roomServer != null);
            
            //will create all room to listen
            //Assert.IsTrue(roomServer.vRoomId == 3);

            this.logger = logger;
        }

        [Action("testfunc_with_body")]
        public TestResponseRet Test([FromPackage] TestRet testRet)
        {
            Debug.Log("TestChannel : Test ");
            Assert.IsTrue(testRet.zProperty.Value == 100);

            var retObj = ZPropertyMesh.CreateObject<TestResponseRet>();
            retObj.zPropertyRet.Value = 200;
            //will return to Caller
            return retObj;
        }

        [Action("testfunc_with_body_and_rawpack")]
        public TestResponseRet FuncWithRawPackage([FromPackage] TestRet testRet, ISocketPackage raw)
        {
            Debug.Log("TestChannel :action testfunc_with_body_and_rawpack ");

            Assert.IsTrue(raw.Key.CompareTo("1001") == 0 || raw.Key.CompareTo("1002") == 0);

            var netPack = NetPackage<TestRet, ZNetErrorEnum>.Parse(raw);
            Assert.IsTrue(netPack.data != null);
            Assert.IsTrue(netPack.data.zProperty.Value == 100);

            var retObj = ZPropertyMesh.CreateObject<TestResponseRet>();
            retObj.zPropertyRet.Value = 200;
            //will return to Caller
            return retObj;
        }

        //send 
        [Action("testfunc_with_valuetype_return_znull")]
        public ZNull FuncWithValueType([FromPackage] int intParam)
        {
            Debug.Log("TestChannel : testfunc_with_valuetype_return_znull called");
            Assert.IsTrue(intParam == 100);
            return ZNull.Default;
        }

        //send with null response to caller
        [Action("testfunc_return_znull")]
        public ZNull Func4()
        {
            Debug.Log("TestChannel : testfunc_return_znull called");

            return ZNull.Default;
        }

        //send with null response to caller
        [Action("testfunc_return_custom_error")]
        public ZNull Func4CustomWithError()
        {
            Debug.Log("TestChannel : testfunc_return_custom_error called");

            //throw new Exception("Error1"); //same as this code
            throw new ZNetMultiException<TestErrorEnum>(TestErrorEnum.Error1);

            return ZNull.Default;
        }

        //send with null response to caller
        [Action("testfunc_return_error")]
        public ZNull Func4WithError()
        {
            Debug.Log("TestChannel : testfunc_return_error called");

            //throw new Exception("null");
            throw new ZNetException(ZNetErrorEnum.ActionError);

            return ZNull.Default;
        }

        //send 
        [Action("testfunc_with_rawdata_return_znull")]
        public ZNull FuncWithRawData([FromPackage] IRawDataPref rawData)
        {
            Debug.Log("TestChannel : testfunc_with_rawdata_return_znull called");
            var rawObj = ZPropertyMesh.CreateObject<TestRet>();
            Assert.IsTrue(rawObj != null);

            ZPropertyPrefs.LoadFromRawData(rawObj, rawData);

            Assert.IsTrue(rawObj.zProperty.Value == 100);
            return ZNull.Default;
        }

        //Post
        [Action("testfunc_with_rawdata_no_return")]
        public void FuncWithRawDataNoReturn([FromPackage] IRawDataPref rawData)
        {
            Debug.Log("TestChannel : testfunc_with_rawdata_no_return called");
            var rawObj = ZPropertyMesh.CreateObject<TestRet>();
            Assert.IsTrue(rawObj != null);

            ZPropertyPrefs.LoadFromRawData(rawObj, rawData);

            Assert.IsTrue(rawObj.zProperty.Value == 100);
        }

        //post
        [Action("testfunc_with_no_return")]
        public void Test2([FromPackage] TestRet testRet)
        {
            Assert.IsTrue(testRet.zProperty.Value == 100);
            Debug.Log("TestChannel : testfunc_with_no_return ");
        }

        //post
        [Action("testfunc_void")]
        public void Test3()
        {
            Debug.Log("TestChannel : testfunc_void called");
        }



    }
}
