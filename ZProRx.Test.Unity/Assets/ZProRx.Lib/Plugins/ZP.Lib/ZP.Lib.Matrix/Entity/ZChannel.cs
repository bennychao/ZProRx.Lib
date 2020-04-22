using System;
using ZP.Lib;
using ZP.Lib.Matrix.Domain;

namespace ZP.Lib.Matrix.Entity
{
    public class ZChannel : IZChannel
    {
        public ZProperty<string> Name = new ZProperty<string>();

        public ZProperty<string> Topic = new ZProperty<string>();

        public ZProperty<ChannelDirectionEnum> Dir = new ZProperty<ChannelDirectionEnum>();

        public ZProperty<ChannelStatusEnum> Status = new ZProperty<ChannelStatusEnum>();

        public ZProperty<ChannelBootFlagEnum> BootFlag = new ZProperty<ChannelBootFlagEnum>();

        public bool IsBroadCast => (BootFlag.Value & ChannelBootFlagEnum.BroadCast) != 0;
        public bool IsMultiCast => (BootFlag.Value & ChannelBootFlagEnum.MultiCast) != 0;
        public bool IsUniCast => (BootFlag.Value & ChannelBootFlagEnum.UniCast) != 0;

        public bool IsRound => (BootFlag.Value & ChannelBootFlagEnum.Round) != 0;

        public bool IsSyncFrame => (BootFlag.Value & ChannelBootFlagEnum.SyncFrame) != 0;

        public bool IsSceneMgr => (BootFlag.Value & ChannelBootFlagEnum.SceneMgr) != 0;

        public bool IsPipeline => (BootFlag.Value & ChannelBootFlagEnum.Pipeline) != 0;


        public bool IsIn => Dir.Value == ChannelDirectionEnum.In;


        public ZChannel()
        {
        }

        public void ChangeDir()
        {
            if (Dir.Value == ChannelDirectionEnum.In)
                Dir.Value = ChannelDirectionEnum.Out;

            else if (Dir.Value == ChannelDirectionEnum.Out)
                Dir.Value = ChannelDirectionEnum.In;
        }


    }
}
