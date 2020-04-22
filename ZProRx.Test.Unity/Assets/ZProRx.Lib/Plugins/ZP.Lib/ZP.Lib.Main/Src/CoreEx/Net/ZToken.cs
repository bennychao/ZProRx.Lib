using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using ZP.Lib.Net;

namespace ZP.Lib.CoreEx
{
    public class ZToken
    {
        private ZProperty<string> accessToken = new ZProperty<string>();
        private ZProperty<ZDateTime> expiresTime = new ZProperty<ZDateTime>();
        private ZProperty<string> refreshToken = new ZProperty<string>();

        private ZProperty<string> redirectData = new ZProperty<string>();

        public string AccessToken => accessToken.Value;

        public string RefreshToken => refreshToken.Value;

        public string RedirectData
        {
            get => redirectData.Value;
            set => redirectData.Value = value;
        }

       static  public ZToken Create(string token)
        {
            var ret = ZPropertyMesh.CreateObject<ZToken>();
            ret.expiresTime.Value = ZDateTime.Now();
            ret.accessToken.Value = token;

            return ret;
        }

        static public ZToken Create(string token, string refreshToken)
        {
            var ret = ZPropertyMesh.CreateObject<ZToken>();
            ret.expiresTime.Value = ZDateTime.Now();
            ret.accessToken.Value = token;
            ret.refreshToken.Value = refreshToken;

            return ret;
        }

        public T GetRedirectData<T>()
        {
            var ret = ZPropertyMesh.CreateObject< NetPackage< T, ZNetErrorEnum> >();
            ZPropertyPrefs.LoadFromStr(ret, redirectData.Value);

            if (ret == null || ret.Error != ZNetErrorEnum.NoError)
                return default(T);

            return ret.Data;
        }

        public bool IsValid()
        {
            return string.IsNullOrEmpty(AccessToken);
        }
    }
}
