using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZP.Lib.Unity.ViewCore;

namespace ZP.Lib.Unity
{

    public class ZUIClickableItem : ZPropertyTriggerItemBehaviour, IZEventItem, IZPropertyViewItem
#if !ZP_SERVER
        , IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
#endif

    {
        public Color NormalColor = Color.gray;
        public Color ClickColor = Color.yellow;

#if !ZP_SERVER
        public IObservable<PointerEventData> OnClickObservable => onClickSubject;
        private Subject<PointerEventData> onClickSubject = new Subject<PointerEventData>();

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
                render.color = ClickColor;
                //Observable.Timer(TimeSpan.FromSeconds(0.8f)).Subscribe(_ => render.color = NormalColor);
            }

            //Event?.Invoke(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var render = GetComponent<Image>();
            if (render != null)
            {
                render.color = NormalColor;
                //Observable.Timer(TimeSpan.FromSeconds(0.8f)).Subscribe(_ => render.color = NormalColor);
            }
            //Event?.Invoke(false);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            var render = GetComponent<Renderer>();
            if (render != null)
            {
                render.material.color = ClickColor;
                Observable.Timer(TimeSpan.FromSeconds(0.8f)).Subscribe(_ => render.material.color = NormalColor);
            }

            zEvent?.Invoke();

            onClickSubject.OnNext(eventData);
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
