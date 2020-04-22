using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using ZP.Lib.Unity.ViewCore;

namespace ZP.Lib.Unity.RTComponents
{
    //bind the IZEvent<bool>
    public class RTHoldable : ZPropertyTriggerItemBehaviour<bool>, IZEventItem<bool>
#if !ZP_SERVER
        , IPointerDownHandler, IPointerUpHandler
#endif
    {
        public Color NormalColor = Color.gray;
        public Color HeightLightColor = Color.white;
        public Color HoldColor = Color.yellow;

#if !ZP_SERVER
        public IObservable<PointerEventData> OnHoldObservable => onHoldSubject;
        private Subject<PointerEventData> onHoldSubject = new Subject<PointerEventData>();

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
        public void OnPointerDown(PointerEventData eventData)
        {
            var render = GetComponent<Renderer>();
            if (render != null)
            {
                render.material.color = HoldColor;
                Observable.Timer(TimeSpan.FromSeconds(0.8f)).Subscribe(_ => render.material.color = NormalColor);
            }

            Event?.Invoke(true);

            onHoldSubject.OnNext(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var rs = GetComponentsInChildren<Renderer>();
            foreach (var r in rs)
            {
                r.material.color = NormalColor;
            }

            Event?.Invoke(false);
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

        // public void OnDisable()
        // {
        //     var rs = GetComponentsInChildren<Renderer>();
        //     foreach (var r in rs)
        //     {
        //         r.material.color = DisableColor;
        //     }
        // }

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
