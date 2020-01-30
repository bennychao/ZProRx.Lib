using System;
namespace ZP.Lib
{
    public class UpdateItem
    {
        public ZProperty<int> ID = new ZProperty<int>();  //index for the reflist
        public ZProperty<UpdateType> UpdateType = new ZProperty<UpdateType>();
        public ZProperty<String> SubPropID = new ZProperty<string>();   //list'item's SubPropID
        public ZProperty<IRawDataPref> RawData = new ZProperty<IRawDataPref>();

        public static UpdateItem AddUpdateItem(object obj)
        {
            var ret = ZPropertyMesh.CreateObject<UpdateItem>();
            ret.RawData.Value = ZPropertyPrefs.ConvertToRawData(obj);

            ret.ID.Value = (int)(obj as IIndexable)?.Index;
            ret.UpdateType.Value = ZP.Lib.UpdateType.Add;

            return ret;
        }

        public static UpdateItem AddUpdateItem(object obj, string subID)
        {
            var ret = ZPropertyMesh.CreateObject<UpdateItem>();
            ret.RawData.Value = ZPropertyPrefs.ConvertToRawData(obj);

            ret.ID.Value = (int)(obj as IIndexable)?.Index;
            ret.UpdateType.Value = ZP.Lib.UpdateType.Add;

            ret.SubPropID.Value = subID;

            return ret;
        }

        public static UpdateItem RemoveUpdateItem(object obj)
        {
            var ret = ZPropertyMesh.CreateObject<UpdateItem>();
            ret.RawData.Value = ZPropertyPrefs.ConvertToRawData(obj);

            ret.ID.Value = (int)(obj as IIndexable)?.Index;
            ret.UpdateType.Value = ZP.Lib.UpdateType.Remove;

            return ret;
        }

        public static UpdateItem RemoveUpdateItem(object obj, string subID)
        {
            var ret = ZPropertyMesh.CreateObject<UpdateItem>();
            ret.RawData.Value = ZPropertyPrefs.ConvertToRawData(obj);

            ret.ID.Value = (int)(obj as IIndexable)?.Index;
            ret.UpdateType.Value = ZP.Lib.UpdateType.Remove;

            ret.SubPropID.Value = subID;

            return ret;
        }

        public static UpdateItem UUpdateItem(object obj, string subID)
        {
            var ret = ZPropertyMesh.CreateObject<UpdateItem>();
            ret.RawData.Value = ZPropertyPrefs.ConvertToRawData(obj);

            ret.ID.Value = (int)(obj as IIndexable)?.Index;
            ret.UpdateType.Value = ZP.Lib.UpdateType.Update;

            ret.SubPropID.Value = subID;

            return ret;
        }

        public static UpdateItem UUpdateItem(object obj)
        {
            var ret = ZPropertyMesh.CreateObject<UpdateItem>();
            ret.RawData.Value = ZPropertyPrefs.ConvertToRawData(obj);

            ret.ID.Value = (int)(obj as IIndexable)?.Index;
            ret.UpdateType.Value = ZP.Lib.UpdateType.Remove;

            return ret;
        }

    }





}
