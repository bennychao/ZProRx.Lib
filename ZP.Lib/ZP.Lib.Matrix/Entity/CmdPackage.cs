using System;
using ZP.Lib;

namespace ZP.Lib.Matrix.Entity
{
    public class CmdPackage<T>
    {
        private ZProperty<T> cmd = new ZProperty<T>();

        private ZProperty<string> organizer = new ZProperty<string>();

        private ZProperty<IRawDataPref> rawData = new ZProperty<IRawDataPref>();

        public T Cmd => cmd.Value;

        public IRawDataPref RawData => rawData.Value;

        public string Organizer
        {
            get => organizer.Value;
            set => organizer.Value = value;
        }

        public CmdPackage()
        {
            //this.cmd.Value = cmd;
        }

        public CmdPackage(T cmd)
        {
            this.cmd.Value = cmd;
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
    }
}
