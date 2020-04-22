using System;
using ZP.Lib;
using ZP.Lib.Server.SQL;

namespace ZP.Lib.Matrix.Entity
{
    public class ZUser
    {
        [DBIndex(true)] //main key
        public ZProperty<uint> UserId = new ZProperty<uint>();

        public ZProperty<string> UserName = new ZProperty<string>();

        public ZProperty<string> Password = new ZProperty<string>();


        public ZProperty<ZDateTime> CreateTime = new ZProperty<ZDateTime>();

        public ZProperty<ZDateTime> LastLoginTime = new ZProperty<ZDateTime>();

        public ZUser()
        {
        }

        public static ZUser Create(string userName, string password)
        {
            var ret = ZPropertyMesh.CreateObject<ZUser>();

            ret.CreateTime.Value.ToNow();

            ret.LastLoginTime.Value.ToNow();

            ret.UserName.Value = userName;
            ret.Password.Value = password;

            return ret;
        }
    }
}
