using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using ZP.Lib.CoreEx;
using ZP.Lib.Core.Relation;
using ZP.Lib.CoreEx.Tools;
using ZP.Lib.Unity.RTComponents;
using ZP.Lib.Main;

namespace ZP.Lib.Core.Values
{

    //inherit from ZHint
    [RTTransformClass(".transform")] //add RTTransfrom
    [PropertyUIItemResClass("[APP]/Hints/", "", ".modelName")]
    //[AddTriggerComponentClass(typeof(RTHoverable), ".OnHold")]
    public class ZStatusHint<TStatusEnum> : ZHint
    {
        //optional
        public ZProperty<TStatusEnum> Status = new ZProperty<TStatusEnum>();
    }
}
