using System;
using ZP.Lib;

namespace ZP.Lib.Matrix.Entity
{
    public class SyncFrameUpdatePackage
    {

        public ZProperty<int> CurFrame = new ZProperty<int>();
        public ZProperty<int> FrameCount = new ZProperty<int>(1);
        public ZProperty<ZListUpdater> Data = new ZProperty<ZListUpdater>();

        public static SyncFrameUpdatePackage Create(int frame)
        {
            var msg = ZPropertyMesh.CreateObject<SyncFrameUpdatePackage>();
            msg.CurFrame.Value = frame;

            return msg;
        }

#if ZP_SERVER
        public static SyncFrameUpdatePackage Create(ZListUpdater updater)
        {
            var msg = ZPropertyMesh.CreateObject<SyncFrameUpdatePackage>();
            msg.Data.Value = updater;
            msg.CurFrame.Value = ZServerScene.Instance.Frame;
            return msg;
        }
#endif

        public void AddUpdateItem(UpdateItem item)
        {
            Data.Value.Add(item);
        }

        
    }


}
