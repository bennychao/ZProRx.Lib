﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Server.CommonTools
{
    static public class ZFunctions
    {
        static public Action NullAction => () => { };

        static public Action<Exception> ThrowNextAction => (Exception e) => throw e;
    }
}
