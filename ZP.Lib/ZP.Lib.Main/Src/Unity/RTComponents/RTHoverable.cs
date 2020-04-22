using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using ZP.Lib.Unity.ViewCore;

namespace ZP.Lib.Main
{
    public class RTHoverable : ZPropertyTriggerItemBehaviour<bool>, IZEventItem<bool>
#if !ZP_SERVER
    // ,IPointerEnterHandler, IPointerUpHandler, IMoveHandler
#endif
    {
        public Color NormalColor = Color.gray;
        public Color HeightLightColor = Color.white;
        public Color HoldColor = Color.yellow;

#if !ZP_SERVER
        public IObservable<Vector3> OnHoverObservable =>
            Observable.Create<Vector3>(observer =>
            {
                var disp = Observable.Interval(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => {
                    Vector3 curPos = Vector3.negativeInfinity;

                    if (CheckHoverPos(out curPos))
                    {
                        observer.OnNext(curPos);

                        //TODO Event.Invoke(true);
                    }
                });

                return disp;
            });

#endif

        //new public bool Bind(IZEvent zEvent)
        //{
        //    Bind(zEvent as IZEvent<bool>);
        //    return true;
        //}

        public bool Bind(IZEvent<bool> zEvent)
        {
            base.BindBase(zEvent);

            return true;
        }
#if !ZP_SERVER

        bool CheckHoverPos(out Vector3 pos)
        {
            var curlayer = gameObject.layer;

            RaycastHit hitInfo;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, 1000, 1 << curlayer))
            {
                pos =  hitInfo.point;
                return true;
            }

            pos =  Vector3.negativeInfinity;
            return false;
        }
         
#endif
        private void OnEnable()
        {
            var rs = GetComponentsInChildren<Renderer>();
            foreach (var r in rs)
            {
                r.material.color = NormalColor;
            }
        }

        public void Unbind()
        {
            base.UnbindBase();
        }


        // Start is called before the first frame update
        void Start()
        {
            var rs = GetComponentsInChildren<Renderer>();
            foreach (var r in rs)
            {
                r.material.color = NormalColor;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
