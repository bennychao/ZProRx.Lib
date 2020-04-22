using System;
namespace ZP.Lib
{
    public class ProCreator<T> where T : ProCreator<T>
    {
        public static T Create()
        {
           return ZPropertyMesh.CreateObject<T>();
        }
    }
}
