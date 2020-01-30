using System;
namespace ZP.Lib
{
    //for the mysql : insert delete udpate
    public enum UpdateType
    {
        Add, //for list
        Remove, //for list
        Update
    }




    public static  class ZPropertyUpdater
    {


        public static void UpdateObject(object obj, ZListUpdater updater)
        {
            foreach (var u in updater.Items)
            {
                var prop = ZPropertyMesh.GetPropertyEx(obj, u.SubPropID);
                if (prop == null || prop.Value == null)
                    continue;

                if (ZPropertyMesh.IsPropertyList(prop))
                {
                    UpdateList(prop as IZPropertyList, ConvertToList( u.RawData.Value));
                }
                else if (ZPropertyMesh.IsPropertyRefList(prop))
                {
                    UpdateList(prop as IZPropertyRefList, ConvertToList( u.RawData.Value));
                }
                else
                {
                    UpdateValue(prop, u.RawData.Value);
                }
            }
        }



        public static void UpdateValue(IZProperty prop, IRawDataPref rawData)
        {
            if (prop == null || prop.Value == null)
                return;

            var sub = ZPropertyMesh.CreateObject(prop.Value.GetType());
            ZPropertyPrefs.LoadFromRawData(sub, rawData);

            prop.Value = sub;
        }

        public static ZListUpdater ConvertToList(IRawDataPref rawData)
        {
            var sub = ZPropertyMesh.CreateObject<ZListUpdater>();
            ZPropertyPrefs.LoadFromRawData(sub, rawData);

            return sub;
        }



        public static void UpdateObject(object obj, string SubPropID, IRawDataPref rawData)
        {
            var prop = ZPropertyMesh.GetPropertyEx(obj, SubPropID);
            if (prop == null || prop.Value == null)
                return;

            UpdateValue(prop, rawData);
        }

        public static void UpdateList<T>(ZPropertyRefList<T> list, ZListUpdater up)
        {
            foreach (var item in up.Items)
            {
                if (item.UpdateType == UpdateType.Update)
                {
                    var prop = list.FindProperty(a => a == item.ID);
                    if (prop == null || prop.Value == null)
                        continue;
                    //UpdateValue(prop, item.RawData.Value);
                    UpdateObject(prop.Value, item.SubPropID, item.RawData.Value);
                }
                else if (item.UpdateType == UpdateType.Add)
                {

                    Type subType = (list as IZProperty).GetDefineType();
                    var newObj = ZPropertyMesh.CreateObject(subType);
                    if (newObj != null)
                    {
                        ZPropertyPrefs.LoadFromRawData(newObj, item.RawData.Value);
                        list.Add(newObj as IIndexable);
                    }
                }
                else if (item.UpdateType == UpdateType.Remove){
                    list.Remove((ZPropertyInterfaceRef<T> v) => v.RefID == item.ID);
                }
            }
        }

        public static void UpdateList<T>(ZPropertyList<T> list, ZListUpdater up)
        {

            Type subType = (list as IZProperty).GetDefineType();

            if (!ZPropertyMesh.IsIndexable(subType)) {
                throw new Exception("list can't update, it's value must be Indexable");
            }

            foreach (var item in up.Items)
            {
                if (item.UpdateType == UpdateType.Update)
                {
                    var prop = list.PropList.Find(a => (a as IIndexable).Index == item.ID);
                    if (prop == null || prop.Value == null)
                        continue;

                    UpdateObject(prop.Value, item.SubPropID, item.RawData.Value);
                }
                else if (item.UpdateType == UpdateType.Add)
                {

                   
                    var newObj = ZPropertyMesh.CreateObject(subType);
                    if (newObj != null)
                    {
                        ZPropertyPrefs.LoadFromRawData(newObj, item.RawData.Value);
                        list.Add((T)newObj);
                    }
                }
                else if (item.UpdateType == UpdateType.Remove){
                    list.Remove((a) => (a as IIndexable).Index == item.ID);
                }
            }
        }

        public static void UpdateList<T>(ZPropertyRefList<T> list, string updateJsonStr)
        {
            var up = ZPropertyMesh.CreateObject<ZListUpdater>();
            ZPropertyPrefs.LoadFromStr(up, updateJsonStr);

            UpdateList<T>(list, up);
        }

        public static void UpdateList<T>(ZPropertyList<T> list, string updateJsonStr)
        {
            var up = ZPropertyMesh.CreateObject<ZListUpdater>();
            ZPropertyPrefs.LoadFromStr(up, updateJsonStr);

            UpdateList<T>(list, up);
        }


        public static void UpdateList(IZPropertyList list, string updateJsonStr)
        {
            var up = ZPropertyMesh.CreateObject<ZListUpdater>();
            ZPropertyPrefs.LoadFromStr(up, updateJsonStr);

            UpdateList(list, up);
        }

        public static void UpdateList(IZPropertyList list, ZListUpdater up)
        {

            Type subType = (list as IZProperty).GetDefineType();

            if (!ZPropertyMesh.IsIndexable(subType))
            {
                throw new Exception("list can't update, it's value must be Indexable");
            }

            foreach (var item in up.Items)
            {
                if (item.UpdateType == UpdateType.Update)
                {
                    var prop = list.PropList.Find(a => (a as IIndexable).Index == item.ID);
                    if (prop == null || prop.Value == null)
                        continue;

                    UpdateObject(prop.Value, item.SubPropID, item.RawData.Value);
                }
                else if (item.UpdateType == UpdateType.Add)
                {


                    var newObj = ZPropertyMesh.CreateObject(subType);
                    if (newObj != null)
                    {
                        ZPropertyPrefs.LoadFromRawData(newObj, item.RawData.Value);
                        list.Add(newObj);
                    }
                }
                else if (item.UpdateType == UpdateType.Remove)
                {
                    list.RemoveObject((a) => (a as IIndexable).Index == item.ID);
                }
            }
        }

        public static void UpdateList(IZPropertyRefList list, string updateJsonStr)
        {
            var up = ZPropertyMesh.CreateObject<ZListUpdater>();
            ZPropertyPrefs.LoadFromStr(up, updateJsonStr);

            UpdateList(list, up);
        }


        public static void UpdateList(IZPropertyRefList list, ZListUpdater up)
        {
            foreach (var item in up.Items)
            {
                if (item.UpdateType == UpdateType.Update)
                {
                    var prop = list.PropList.Find(a => a == item.ID);
                    if (prop == null || prop.Value == null)
                        continue;
                    //UpdateValue(prop, item.RawData.Value);
                    UpdateObject(prop.Value, item.SubPropID, item.RawData.Value);
                }
                else if (item.UpdateType == UpdateType.Add)
                {

                    Type subType = (list as IZProperty).GetDefineType();
                    var newObj = ZPropertyMesh.CreateObject(subType);
                    if (newObj != null)
                    {
                        ZPropertyPrefs.LoadFromRawData(newObj, item.RawData.Value);
                        list.Add(newObj as IIndexable);
                    }
                }
                else if (item.UpdateType == UpdateType.Remove)
                {
                    list.RemoveObject(v => (v as IRefable).RefID == item.ID);
                }
            }
        }
    }
}
