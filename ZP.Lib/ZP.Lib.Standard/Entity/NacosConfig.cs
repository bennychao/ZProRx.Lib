using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Standard.Entity
{
    public class NacosConfig
    {
        public string ServerAddresses { get; set; } = "49.233.89.69:8848";
        public int DefaultTimeOut { get; set; } = 8;

        public string Namespace { get; set; } = "";

        public string Group { get; set; } = "Nacos";
    }
}
