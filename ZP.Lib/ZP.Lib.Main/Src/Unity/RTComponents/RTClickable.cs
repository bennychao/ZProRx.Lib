
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !ZP_SERVER
using UnityEngine.EventSystems;
#endif

using UniRx;
using System;
using ZP.Lib.Unity.ViewCore;

namespace ZP.Lib
{
    public class RTClickable : ZPropertyTriggerItemBehaviour, IZEventItem, IZPropertyViewItem
#if !ZP_SERVER
        , IPointerExitHandler, IPointerClickHandler, IPointerEnterHandler
#endif
    {
        public Color NormalColor = Color.gray;
        public Color HeightLightColor = Color.white;
        public Color ClickColor = Color.yellow;
        public Color DisableColor = Color.grey * 0.5f;

#if !ZP_SERVER
        public IObservable<PointerEventData> OnClickObservable => onClickSubject;
        private Subject<PointerEventData> onClickSubject = new Subject<PointerEventData>();

#endif


#if !ZP_SERVER
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            var rs = GetComponentsInChildren<Renderer>();
            foreach (var r in rs)
            {
                r.material.color = HeightLightColor;
            }

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var rs = GetComponentsInChildren<Renderer>();
            foreach (var r in rs)
            {
                r.material.color = NormalColor;
            }
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

        public void OnDisable()
        {
            var rs = GetComponentsInChildren<Renderer>();
            foreach (var r in rs)
            {
                r.material.color = DisableColor;
            }
        }

        public void Unbind()
        {

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

