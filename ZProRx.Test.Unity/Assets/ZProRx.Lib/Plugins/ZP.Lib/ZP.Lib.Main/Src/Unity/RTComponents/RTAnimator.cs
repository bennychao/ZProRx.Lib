using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZP.Lib.Main;
using UniRx;

namespace ZP.Lib
{
    public class RTAnimator : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
#if !ZP_SERVER
        Animator animator = null;
#endif
        public bool Bind(IZProperty property)
        {
            base.BindBase(property);
#if !ZP_SERVER
            animator = GetComponentInChildren<Animator>();

            var aparams = animator.parameters.ToList();

            var props = ZPropertyMesh.GetProperties(property.Value);

            var events = ZPropertyMesh.GetEvents(property.Value);

            foreach (var ap in aparams)
            {

                //bind triggers
                if (ap.type == AnimatorControllerParameterType.Trigger)
                {
                    var evlink = events.Find(e =>
                    {
                        var tag = ZPropertyMesh.GetTag(e, TagNameSpace.UnityAnimatorParameters);

                        return string.Compare(ap.name, tag) == 0;
                    });

                    if (evlink != null)
                    {
                        evlink.OnEventObservable().Subscribe(_ =>
                        {
                            animator.SetTrigger(ap.name);
                        });
                    }

                    continue;
                }

                //bind params
                var link = props.Find(p =>
               {
                   var tag = ZPropertyMesh.GetTag(p, TagNameSpace.UnityAnimatorParameters);

                   return string.Compare(ap.name, tag) == 0;
               });

                if (link != null)
                {
                    link.ValueChangeAsObservable().ObserveOnMainThread().Subscribe(v =>
                    {
                        SetParam(ap, v);
                    });

                    //update current value
                    SetParam(ap, link.Value);
                    continue;
                }

            }

#endif
            return true;
        }

#if !ZP_SERVER
        private void SetParam(AnimatorControllerParameter ap, object v)
        {
            switch (ap.type)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(ap.name, Convert.ToBoolean(v));
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(ap.name, Convert.ToSingle(v));
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(ap.name, Convert.ToInt32(v));
                    break;
            }
        }
#endif


        public void Unbind()
        {
            base.UnbindBase();
        }

        public void UpdateValue(object data)
        {
            throw new NotImplementedException();
        }
    }
}
