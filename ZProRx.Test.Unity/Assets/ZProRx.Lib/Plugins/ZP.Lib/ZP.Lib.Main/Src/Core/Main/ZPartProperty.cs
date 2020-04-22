using System;
using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    public class ZPartProperty<T> : ZProperty<T>, IZPartProperty
    {
        private string partName = "";

        public string PartName { get => partName; set=> partName = value; }

        public bool IfHasPartName()
        {
            return String.IsNullOrEmpty(partName);
        }
    }
}

