using System;
using System.Collections.Generic;

namespace ZP.Lib
{


    /// <summary>
    /// ZP roperty interface reference.
    /// </summary>
    public class ZPropertyInterfaceRef<T> : ZProperty<T>, IRefable, IDelayValuable<T>
    {
        protected int refID;
        public RefBindEvent OnBindProp;
        public delegate T RefBindEvent(int id);

        public ZPropertyInterfaceRef(RefBindEvent onbind)
        {
            this.OnBindProp = onbind;
        }

        public ZPropertyInterfaceRef(int refid, RefBindEvent onbind)
        {
            this.refID = refid;
            //this.Value = data;
            this.OnBindProp = onbind;
        }

        public ZPropertyInterfaceRef()
        {
            this.OnBindProp = null;
        }

        public int RefID
        {
            get
            {
                return refID;
            }

            set
            {
                refID = value;

                //rebind
                BindRef();
            }
        }



        public bool IsReady()
        {
            return Value != null;
        }

        public override void CopyData(IZProperty prop)
        {
            var ip = prop as ZPropertyInterfaceRef<T>;
            if (ip == null)
                return;

            refID = ip.RefID;
            OnBindProp = ip.OnBindProp;
            base.CopyData(prop);
        }

        protected override T GetValue()
        {
            if (data == null)
                BindRef();

            return base.GetValue();
        }

        public void BindRef()
        {
            //throw new NotImplementedException();
            // var attr = ZPropertyAttributeTools.GetAttribute<PropertyRefBindDelegateAttribute>(this as IZProperty);
            //if (attr != null)
            //{
            //    if ( attr.onBind != null)
            //    {
            //        this.Value = (T)(attr.onBind());
            //    }
            //}

            if (OnBindProp != null)
            {
                this.Value = OnBindProp(RefID);
            }
        }


        /// <summary>
        /// ZLs the ib. IR efable. to map.
        /// </summary>
        /// <returns>The ib. IR efable. to map.</returns>
        object IRefable.ToMap()
        {
            //not return the interface
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret["refid"] = refID;
            return ret;
        }

        /// <summary>
        /// Raises the load event. Zproperty will not be called when onload ,is the value.
        /// </summary>
        //public void OnLoad()
        //{
        //    BindRef();
        //}
    }
}

