using System;
using System.Collections.Generic;
using System.Reflection;
using LitJson;
using UnityEngine;

namespace ZP.Lib
{
    public class UpdateJsonItem {
        public int ID;
        public string PropID; //multi support
        public UpdateType UpdateType;
        public JsonData Data;
    }

    public class ListJsonUpdater {
        public List<UpdateJsonItem> Items = new List<UpdateJsonItem>();
    }

    public static class ZPropertyJsonLitUpdater
    {
        static public bool UpdateList<T>(ZPropertyRefList<T> list, string updateJsonStr) {

            JsonData data = JsonMapper.ToObject(updateJsonStr);

            ListJsonUpdater updater = ZPropertyMesh.CreateObject<ListJsonUpdater>();

            ConvertArrayToListUpdater(updater.Items, data["Items"]);

            bool bRet = updater.Items.Count > 0;

            foreach (var item in updater.Items) {
                var itemProp = list.FindProperty(a => a == item.ID);

                if (itemProp == null || itemProp.Value == null)
                {
                   // bRet = false;
                    continue;
                }

                var updateProp = ZPropertyMesh.GetPropertyEx(itemProp.Value, item.PropID);
                if (updateProp == null || updateProp.Value == null)
                {
                   // bRet = false;
                    continue;
                }

                var jsonRrefs = ZPropertyPrefs.GetRrefs() as ZPropertyJsonLitPrefs;

                if (item.UpdateType == UpdateType.Update)
                {
                    jsonRrefs.ConvertToObject(updateProp.Value, item.Data);
                }
                else if (item.UpdateType == UpdateType.Add){

                    Type subType = (list as IZProperty).GetDefineType();
                    var newObj = ZPropertyMesh.CreateObject(subType);
                    if (newObj != null)
                    {
                        jsonRrefs.ConvertToObject(newObj, item.Data);

                        list.Add(newObj as IIndexable);
                    }
                }
                else if (item.UpdateType == UpdateType.Remove){
                    list.Remove((ZPropertyInterfaceRef<T> v) => v.RefID == item.ID);
                }

            }

            return bRet;
        }

        static internal void InvokeUpdateMethod(object obj)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod("OnUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);      // 获取方法信息
            object[] parameters = null;

            if (method != null)
                method.Invoke(obj, parameters);                           // 调用方法，参数为空
        }

        static private void ConvertArrayToListUpdater(List<UpdateJsonItem> Items, JsonData data)
        {
            if (Items != null)
            {
                foreach (JsonData data1 in data)
                {
                    var sub = new UpdateJsonItem();
                    sub.ID = (int)data1["ID"];
                    sub.PropID = (string)data1["PropID"];
                    sub.UpdateType = (UpdateType)(int)data1["UpdateType"];
                    sub.Data = data1["Data"];
                    Items.Add(sub);
                }
            }
        }
    }
}
