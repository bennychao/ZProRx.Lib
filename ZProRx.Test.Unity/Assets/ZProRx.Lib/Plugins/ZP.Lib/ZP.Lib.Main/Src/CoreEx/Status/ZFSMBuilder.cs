using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;

namespace ZP.Lib.CoreEx.Status
{
    public class ZFSMBuilder<TFSM, S, E> : ZFSM<S, E> 
        where TFSM : ZFSMBuilder<TFSM, S, E>
        where S : IComparable
        where E : IComparable
    {
        public static TFSM CreateFSM()
        {
            var zFSM = ZPropertyMesh.CreateObject<TFSM>();

            zFSM.StatusType = typeof(ZStatusProperty<S, E>.ZPropStatus);

            return zFSM;
        }
    }
}
