using System;
using ZP.Lib;

namespace ZP.Lib.Matrix.Entity
{
    sealed public class ZRoomRole
    {
        private ZProperty<string> clientId = new ZProperty<string>();
        private ZProperty<int> roleId = new ZProperty<int>();    //can be user as timeId

        private ZProperty<bool> bComputer = new ZProperty<bool>();

        public string ClientId => clientId.Value;
        public int RoleId => roleId.Value;

        public bool IsComputer => bComputer.Value;
    }
}
