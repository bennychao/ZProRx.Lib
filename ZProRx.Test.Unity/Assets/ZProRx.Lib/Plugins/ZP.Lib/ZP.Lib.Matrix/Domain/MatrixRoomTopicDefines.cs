using System;
using System.Collections.Generic;
using System.Text;

namespace ZP.Lib.Matrix.Domain
{
    public static class MatrixRoomTopicDefines
    {
        public static string RoomConnect(int vroomId) => string.Format("matrix/hall/connect/{0}", vroomId);

        public static string RoomReconnect(int vroomId) => string.Format("matrix/hall/reconnect/{0}", vroomId);

        public static string RoomDisConnect(int vroomId) => string.Format("matrix/hall/disconnect/{0}", vroomId);
    }
}
