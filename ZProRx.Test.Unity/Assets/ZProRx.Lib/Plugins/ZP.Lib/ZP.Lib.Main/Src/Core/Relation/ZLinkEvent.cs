using System.Collections;
using System.Collections.Generic;

namespace ZP.Lib
{
    public class ZLinkEvent : ZEvent, IZLinkable
    {
        private IZProperty linkProperty = null;

        public IZProperty LinkProperty {
            get => linkProperty;
            set {
                linkProperty = value;
                OnLinked();
            }
        }

        protected virtual void OnLinked()
        {

        }


        public override void Invoke()
        {
            if (LinkProperty != null)
            {
                var act = LinkProperty.Value as IZEventAction;
                act?.OnEvent(this);
            }

            base.Invoke();
        }
    }

    public class ZLinkEvent<T> : ZEvent<T>, IZLinkable
    {
        private IZProperty linkProperty = null;

        public IZProperty LinkProperty
        {
            get => linkProperty;
            set
            {
                linkProperty = value;
                OnLinked();
            }
        }
        protected virtual void OnLinked()
        {

        }

        public override void Invoke(T data)
        {
            if (LinkProperty != null)
            {
                var act = LinkProperty.Value as IZEventAction<T>;
                act?.OnEvent(this, (T)data);
            }

            base.Invoke(data);
        }
    }
}

