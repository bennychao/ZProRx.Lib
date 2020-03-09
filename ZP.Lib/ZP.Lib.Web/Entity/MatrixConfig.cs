using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Matrix.Domain;
using ZP.Lib.Web.Domain;

namespace ZP.Lib.Web.Entity
{
    public class MatrixWebConfig : IMatrixConfig
    {
        public string ServerName { get; set; }
        public string AuthorizationServer { get; set; }
        public string ArchitectMasterServer { get; set; }
        public float IdleDeadline { get; set; }
        public string StockmanMasterServer { get; set; }

        public string StewardMasterServer { get; set; }

        public string MerchantMasterServer { get; set; }

        public string SupervisorMasterServer { get; set; }
        public MatrixWebConfig(IConfiguration configuration)
        {
            //configuration.GetSection()
            var section = configuration.GetSection("MatrixConfig");
            section.Bind(this);


        }
    }
}
