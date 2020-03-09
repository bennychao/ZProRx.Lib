using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix.Domain
{
    [AttributeUsage( AttributeTargets.Method, Inherited = true)]
    public class ActionAttribute : Attribute
    {
        public string description;
        public string template;
        public ActionAttribute(string template, string description = "")
        {
            this.template = template;
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class PreListenAttribute : Attribute
    {
        public PreListenAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = true)]
    public class FromPackageAttribute : Attribute
    {
        public string description;
        public string template;
        public FromPackageAttribute()
        {
        }
    }

    [Flags]
    public enum ChannelBootFlagEnum
    {
        Normal = 0,
        ClientSuite = 0x01,     //one channel for one client
        RoomSuite = 0x02,   //one channel for all room's  client
        //Suit = 0x02,     //one channel for one client
       
        AutoEnable = 0x04,
        ManualCreate = 0x08, //not create auto to add channellist

        //mutually Selection
        BroadCast = 0x10,
        MultiCast = 0x20,
        UniCast = 0x40,
        Round = 0x80,
        SyncFrame = 0x100,
        SceneMgr = 0x200,

        Pipeline = 0x1000,  //for custom pipeline, not support the 
        //Cast
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ChannelBootAttribute : Attribute
    {
        public ChannelBootFlagEnum BootFlag { get; set; }
        public ChannelBootAttribute(ChannelBootFlagEnum bootFlag)
        {
            this.BootFlag = bootFlag;
            if (CheckSupport())
            {
                throw new Exception("one support set one Cast mode");
            }
        }

       

        public bool Support(ChannelBootFlagEnum flag)
        {
            return (BootFlag & flag) != 0;
        }

        //public bool Support(ChannelBootFlagEnum flag)
        //{
        //    return (BootFlag & flag) != 0;
        //}

        private bool CheckSupport()
        {
            var cu =(UInt32) this.BootFlag & 0x70 >> (int)ChannelBootFlagEnum.BroadCast;
            //check only has one bit is 1
            int oneCount = 0;
            for (int i = 0; i < 4; i++, cu = cu>> 1)
            {
                if ((cu & 0x01) !=  0)
                    oneCount++;
            }

            return oneCount > 1;
        }
    }


    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class SyncResultAttribute : Attribute
    {
        int timeout = 30000; 
        public int Timeout => timeout;
        public SyncResultAttribute(int timeout = 30000) //u: ms time out is 0 to wait only
        {
            this.timeout = timeout;
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class SocAuthorizeAttribute : Attribute
    {
        public SocAuthorizeAttribute() //u: ms time out is 0 to wait only
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ActionFilterAttribute : Attribute
    {
        readonly Type filterType;
        public Type FilterType => filterType;
        public ActionFilterAttribute(Type filterType) //u: ms time out is 0 to wait only
        {
            this.filterType = filterType;
        }
    }
}
