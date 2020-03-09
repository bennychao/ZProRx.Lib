using System;
using ZP.Lib;

namespace ZP.Lib.Matrix.Entity
{
    public class SyncFrameActionPackages<TAction>
    {
        public ZPropertyList<SyncFrameActionPackage<TAction>> Actions
            = new ZPropertyList<SyncFrameActionPackage<TAction>>();

        public ZProperty<int> CurFrame = new ZProperty<int>();

        public void Clear()
        {
            Actions.ClearAll();
        }
    }


    sealed public class SyncFrameActionPackage<TAction>
    {
        private ZProperty<TAction> action = new ZProperty<TAction>();
        private ZProperty<string> organizer = new ZProperty<string>();

        private ZProperty<IRawDataPref> rawData = new ZProperty<IRawDataPref>();

        public TAction Action
        {
            get => action.Value;
            set => action.Value = value;
        }

        public string ClientId
        {
            get => organizer.Value;
            set => organizer.Value = value;
        }

        public IRawDataPref RawData => rawData.Value;


        public SyncFrameActionPackage(TAction action)
        {
            this.action.Value = action;
        }

        public TData GetData<TData>()
        {
            var data = ZPropertyMesh.CreateObject<TData>();
            ZPropertyPrefs.LoadFromRawData(data, rawData.Value);

            return data;
        }
        public void SetData<TData>(TData data)
        {
            rawData.Value = ZPropertyPrefs.ConvertToRawData(data);
        }

        public void SetData(object data)
        {
            rawData.Value = ZPropertyPrefs.ConvertToRawData(data);
        }

        public static SyncFrameActionPackage<TAction> CreateFrameStr(string strData)
        {
            var frame = ZPropertyMesh.CreateObject<SyncFrameActionPackage<TAction>>();
            ZPropertyPrefs.LoadFromStr(frame, strData);

            return frame;
        }

    }
}
