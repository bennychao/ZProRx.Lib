using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib.Matrix.Entity;
using ZP.Lib.SocClient.Domain;

namespace ZP.Lib.SocClient
{
   public static   class SocClientBuildingExtensions
    {
        static public ISocClientBuilder AddPipeline<T>(this ISocClientBuilder socRoomBuilder) where T : BasePipeline
        {
            socRoomBuilder.AddChannelType<T>();

            //TODO check boot attribute

            return socRoomBuilder;
        }

    }
}
