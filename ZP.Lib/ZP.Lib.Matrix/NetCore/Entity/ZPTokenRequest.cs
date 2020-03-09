using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.NetCore.Entity
{
    public class ZPTokenRequest
    {
        private ZProperty<string> grantType = new ZProperty<string>();

        private ZProperty<string> clientId = new ZProperty<string>();

        private ZProperty<string> clientSecret = new ZProperty<string>();

        private ZProperty<string> userName = new ZProperty<string>();

        private ZProperty<string> password = new ZProperty<string>();

        public string GrantType => grantType.Value;
        public string ClientId => clientId.Value;
        public string ClientSecret => clientSecret.Value;
        public string UserName => userName.Value;
        public string Password => password.Value;
    }
}
