using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Soc;

namespace ZP.Lib.Soc.Entity
{
    internal class MatrixSocConfig : IMatrixConfig
    {
        public string ServerName { get; set; }
        public string AuthorizationServer { get; set; }

        public string ArchitectMasterServer { get; set; }

        public float IdleDeadline { get; set; }
        public string StockmanMasterServer { get; set; }

        public string StewardMasterServer { get; set; }

       public  string MerchantMasterServer { get; set; }

        public  string SupervisorMasterServer { get; set; }

        public MatrixSocConfig(IConfiguration configuration)
        {
            //configuration.GetSection()
            var section = configuration.GetSection("MatrixConfig");
            section.Bind(this);
        }
    }
}
