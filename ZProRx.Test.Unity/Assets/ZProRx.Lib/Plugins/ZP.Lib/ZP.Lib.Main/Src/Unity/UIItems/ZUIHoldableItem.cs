using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

using UnityEngine.UI;
using ZP.Lib;
using ZP.Lib.Unity.ViewCore;

#if !ZP_SERVER
using UnityEngine.EventSystems;
#endif

namespace ZP.Lib.Unity
{

    public class ZUIHoldableItem : ZPropertyTriggerItemBehaviour<bool>, IZEventItem<bool>,  IZPropertyViewItem
#if !ZP_SERVER
        , IPointerDownHandler, IPointerUpHandler
#endif

    {
        public Color NormalColor = Color.gray;
        public Color HoldColor = Color.yellow;

#if !ZP_SERVER
        public IObservable<PointerEventData> OnHoldObservable => onHoldSubject;
        private Subject<PointerEventData> onHoldSubject = new Subject<PointerEventData>();

#endif

        //only for Event Bind
        public bool Bind(IZEvent<bool> zEvent)
        {
            base.BindBase(zEvent);

            return true;
        }

#if !ZP_SERVER
        public void OnPointerDown(PointerEventData eventData)
        {
            var render = GetComponent<Image>();
            if (render != null)
            {
                render.color = HoldColor;
                //Observable.Timer(TimeSpan.FromSeconds(0.8f)).Subscribe(_ => render.color = NormalColor);
            }

            Event?.Invoke(true);

            onHoldSubject.OnNext(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var render = GetComponent<Image>();
            if (render != null)
            {
                render.color = NormalColor;
                //Observable.Timer(TimeSpan.FromSeconds(0.8f)).Subscribe(_ => render.color = NormalColor);
            }
            Event?.Invoke(false);
        }

        private void OnEnable()
        {
            var render = GetComponent<Image>();
            if (render != null)
            {
                render.color = NormalColor;
            }
        }
#endif

        public void Unbind()
        {
            base.UnbindBase();
        }


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
