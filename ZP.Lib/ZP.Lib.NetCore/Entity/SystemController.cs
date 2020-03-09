using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib;
using ZP.Lib.Net;
using ZP.Lib.NetCore.Domain;
using ZP.Lib.Web;

namespace ZP.Lib.NetCore
{
    /// <summary>
    /// For web host control
    /// </summary>
    [Route("api/v1/com/[controller]")]
    [ApiController]

    public class SystemController : ControllerBase
    {
        IWebHostController webHostController = null;
        public SystemController(IWebHostController webHostController)
        {
            this.webHostController = webHostController;
        }


        /// <summary>
        /// Remove the RoomContainer and it's rooms
        /// </summary>
        /// <returns></returns>
        [HttpDelete("shutdown")]
        public ZsonResult<ZNull> Shutdown()
        {
            if (webHostController?.WebHost == null)
            {
                return ZNetErrorEnum.UnsupportedApi;
            }

            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(
                _ =>
                {
                    webHostController?.Shutdown();
                });

            return ZNull.Default;
        }


    }
}
