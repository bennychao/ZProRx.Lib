using System;
using ZP.Lib;

namespace ZP.Lib.Matrix.Entity
{
    public class ZUserRegisterRequest
    {
        private ZProperty<string> userName = new ZProperty<string>();
        private ZProperty<string> password = new ZProperty<string>();

        private ZProperty<IRawDataPref> registerData = new ZProperty<IRawDataPref>();

        public string UserName => userName.Value;
        public string Password => password.Value;

        public ZUserRegisterRequest()
        {
        }

        public ZUserRegisterRequest(ZUser user)
        {
            userName.Value = user.UserName;
            password.Value = user.Password;
        }

        public ZUserRegisterRequest(string userName, string password)
        {
            this.userName.Value = userName;
            this.password.Value = password;
        }

        public TData GetData<TData>()
        {
            var data = ZPropertyMesh.CreateObject<TData>();
            ZPropertyPrefs.LoadFromRawData(data, registerData.Value);

            return data;
        }

        public void SetData<TData>(TData data)
        {
            registerData.Value = ZPropertyPrefs.ConvertToRawData(data);
        }

        public void SetData(object data)
        {
            registerData.Value = ZPropertyPrefs.ConvertToRawData(data);
        }
    }
}
