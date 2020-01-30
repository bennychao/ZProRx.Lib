using System;
namespace ZP.Lib
{
    public class UploadItem<T>
    {
        public ZProperty<int> ID = new ZProperty<int>();
        public ZProperty<UpdateType> UpdateType = new ZProperty<UpdateType>();
        public ZProperty<String> SubPropID = new ZProperty<string>();
        public ZProperty<T> Value = new ZProperty<T>();
    }

}
