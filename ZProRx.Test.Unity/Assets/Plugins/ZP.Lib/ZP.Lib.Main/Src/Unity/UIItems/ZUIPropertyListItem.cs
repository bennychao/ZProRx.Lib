
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;


#if ZP_UNITY_CLIENT

#endif

using System.Linq;
using ZP.Lib.CoreEx.Reactive;

namespace ZP.Lib
{
    public class ZUIPropertyListItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
        public IObservable<IZProperty> OnClickItemObservable => onClickItemSubject;

        private Subject<IZProperty> onClickItemSubject = new Subject<IZProperty>();
        private Transform root;

        private object lockObj = new object();
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public  bool Bind(IZProperty property)
        {
            base.BindBase(property);
//#if ZP_UNITY_CLIENT
            ////register button event
            root = ZViewBuildTools.FindComponentInChildren<Transform>(this.transform, ZViewCommonObject.Root);

            if (root == null)
                root = transform;

            if (!ZPropertyMesh.IsPropertyListLike(property))
            {
                Debug.LogError("property value is Linkable but it is not a ZpropertyList<String> Link");

                return false;
            }

            //for common IZPropertyList
            var list = property as IZPropertyList;
            if (list != null)
            {
                list.AddItemAsObservable().Subscribe(chprop =>
                {
                    lock (lockObj)
                    {
                        ZViewBuildTools.BindProperty(chprop as IZProperty, root);
                    }
                }).AddTo(disposables);

                //init the dead status
                disposables.Add(new OnDeleteItemDisposable(list, (delProp, index) =>
                {
                    lock (lockObj)
                    {
                        Transform tran = delProp.TransNode;

                        if (tran != null)
                        {
                            ObservableEx.NextFrame().Subscribe(_=> GameObject.DestroyImmediate(tran.gameObject));
                            //GameObject.DestroyImmediate(tran);
                        }
                    }

                }));

                //bind the item click message
#if ZP_UNITY_CLIENT
                list.PropList
                .Select(item => (btn: item.TransNode.GetComponentInChildren<Button>() as Button, prop: item)).ToList()
                .ForEach(item => {
                    item.btn.onClick.ToObservable().Subscribe(_ => onClickItemSubject.OnNext(item.prop));

                });
#endif
            }

            //for IZPropertyRefList
            var reflist = property as IZPropertyRefList;
            if (reflist != null)
            {
                reflist.AddItemAsObservable().Subscribe(chprop =>
                {
                    lock (lockObj)
                    {
                        ZViewBuildTools.BindProperty(chprop as IZProperty, root);
                    }
                        
                }).AddTo(disposables);

                //init the dead status
                disposables.Add(new OnDeleteRefItemDisposable(reflist,  (delProp, index) =>
                {
                    lock (lockObj)
                    {
                        Transform tran = delProp.TransNode;

                        if (tran != null)
                        {
                            ObservableEx.NextFrame().Subscribe(_=> GameObject.DestroyImmediate(tran.gameObject));
                            //GameObject.DestroyImmediate(tran.gameObject);
                        }
                    }
                }));

#if ZP_UNITY_CLIENT
                reflist.PropList
                .Select(item => (btn: item.TransNode.GetComponentInChildren<Button>() as Button, prop: item)).ToList()
                .ForEach(item => {
                   item.btn.onClick.ToObservable().Subscribe(_ => onClickItemSubject.OnNext(item.prop));

                });
#endif

            }

            //set the List's sizeTODO
            //var btnProps = gameObject.GetComponentsInChildren(typeof(Button));

//#endif
            return true;
        }

        //public Transform GetNewItem()
        //{
        //    //transform.GetSiblingIndex()
        //}

        public void UpdateValue(object data)
        {

        }

        public void ClearRoot()
        {
            root?.DetachChildren();
        }

        public new void Unbind()
        {
            base.UnbindBase();
            //throw new System.NotImplementedException();

            //TODO release the event OnAddItem ex.
        }
    }
}

