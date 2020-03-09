using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix.Domain
{
    public interface IMatrixConfig
    {
        string ServerName { get; set; }
        string AuthorizationServer { get;  }

        string ArchitectMasterServer { get; }
        string StockmanMasterServer { get; set; }

        string StewardMasterServer { get; set; }

        string MerchantMasterServer { get; set; }

        string SupervisorMasterServer { get; set; }

        float IdleDeadline { get; set; }
    }
}
