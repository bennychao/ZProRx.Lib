using ZP.Lib.Common;
using System;
namespace ZP.Lib.Net
{
    public enum ZNetErrorEnum
    {
        NoError = 0,

        BaseError = 0x5000,
        ActionError,

        [EnumStr("不支持的API")]
        UnsupportedApi,

        UnsupportedTarget,

        UnsupportedParam,

        UnsupportedAction,

        [EnumStr("没有进行授权")]
        UnAuthorized,

        [EnumStr("没有足够的空间")]
        NotEnoughMoney,

        NotEnoughCard,

        NotEnoughMaterial,

        [EnumStr("")]
        NotFind,

        [EnumStr("超时")]
        Timeout,

        [EnumStr("")]
        UnDefineGroup,

        [EnumStr("")]
        CreatePlayerError,

        [EnumStr("")]
        PlayerAlreadyExists,

        [EnumStr("")]
        UserAlreadyExists,

        [EnumStr("")]
        CustomError,

        MaxError = 0x6000
    }
}
