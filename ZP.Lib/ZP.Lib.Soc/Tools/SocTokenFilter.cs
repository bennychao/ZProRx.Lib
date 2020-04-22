using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ZP.Lib.CoreEx;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.Net;

namespace ZP.Lib.Soc.Tools
{
    class SocTokenFilter<T> : IActionFillter<T>
    {
        private static readonly string secretKey = "mysupersecret_secretkey!123";


        BaseChannel baseChannel;

        TokenValidationParameters option = new TokenValidationParameters
        {
            // RequireSignedTokens = false,
            NameClaimType = JwtClaimTypes.Name,
            RoleClaimType = JwtClaimTypes.Role,
            //ValidateIssuerSigningKey = false,
            ValidIssuer = "testzb",
            ValidAudience = "api",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))

            /***********************************TokenValidationParameters的参数默认值***********************************/
            // RequireSignedTokens = true,
            // SaveSigninToken = false,
            // ValidateActor = false,
            // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
            // ValidateAudience = true,
            // ValidateIssuer = true, 
            // ValidateIssuerSigningKey = false,
            // 是否要求Token的Claims中必须包含Expires
            // RequireExpirationTime = true,
            // 允许的服务器时间偏移量
            // ClockSkew = TimeSpan.FromSeconds(300),
            // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
            // ValidateLifetime = true
        };

        public SocTokenFilter (BaseChannel baseChannel)
        {
            this.baseChannel = baseChannel;
        }

        public SocketPackageHub<T> OnFilter(SocketPackageHub<T> obj)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string token = "";
            obj.data.Headers.TryGetValue("token", out token);

            SecurityToken securityToken = null;
            var claims = tokenHandler.ValidateToken(token, option, out securityToken);
            var roomIdClaim = claims?.FindFirst("roomId");

            var rid = (roomIdClaim != null ? Convert.ToInt16(roomIdClaim.ToString()) : -1);
            //Assert.Equals(rid, )

            if (rid != baseChannel.RoomId)
            {
                throw new ZNetException(ZNetErrorEnum.UnAuthorized);
            }

            return obj;
        }
    }
}
