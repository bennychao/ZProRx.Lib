using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Common
{
   public class ObjectLock
    {
        System.Boolean bLock = false;

        public bool GetLock()
        {
            bool CanLock = false;
            lock (this)
            {
                if (!bLock)
                {
                    bLock = true;
                    CanLock = true;
                }
            }

            return CanLock;
        }

    }
}
