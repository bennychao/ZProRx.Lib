using System;
using System.Collections.Generic;
using System.Text;
using ZP.Lib;
using UniRx;

namespace ZP.Lib.Unity
{
    public class ZUIDialogView : ZPropertyViewRootBehaviour, IZPropertyViewRoot
    {
        protected MultiDisposable disposables = new MultiDisposable();
        public void Bind(object root)
        {
            base.BindBase(root);

            //bind the Button Event
            var onClose = ZPropertyMesh.GetEventEx(root, ".OnClose");

             onClose.OnEventObservable().Subscribe(_ =>
            {
                this.transform.gameObject.SetActive(false);

                ZViewBuildTools.UnBindObject(root, this.transform);
            }).AddTo(disposables);
        }

        new public void Unbind()
        {
            base.Unbind();
            disposables.Dispose();
        }
    }
}
