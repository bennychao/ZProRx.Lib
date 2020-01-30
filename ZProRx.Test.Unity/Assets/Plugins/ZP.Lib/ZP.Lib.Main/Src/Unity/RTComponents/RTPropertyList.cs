
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ZP.Lib
{
    public class RTPropertyList : ZPropertyViewItemBehaviour, IZPropertyViewItem
    {
        private object lockObj = new object();

        //private MultiDisposable disposables = new MultiDisposable();

        Transform root;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool Bind(IZProperty property)
        {
            base.BindBase(property);

            ////register button event
            root = ZViewBuildTools.FindComponentInChildren<Transform>(this.transform, ZViewCommonObject.Root);

            if (root == null)
                root = transform;

            if (!ZPropertyMesh.IsPropertyListLike(property))
            {
                Debug.LogError("property value  is not a ZpropertyList<String> Link");

                return false;
            }

            var list = property as IZPropertyList;
            if (list != null)
            {
                list.AddItemAsObservable().Subscribe( chprop =>
                {
                    lock (lockObj)
                    {
                        ZViewBuildTools.BindProperty(chprop as IZProperty, root);
                    }
                }).AddTo(disposables);

                //init the dead status
                disposables.Add(new OnDeleteItemDisposable( list,  (delProp, index) =>
                {
                    lock (lockObj)
                    {
                        Transform tran = delProp.TransNode;

                        if (tran != null)
                        {
                            DestroyImmediate(tran);
                        }
                    }

                }));
            }

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
                disposables.Add( new OnDeleteRefItemDisposable(reflist, (delProp, index) =>
                {
                    lock (lockObj)
                    {
                        Transform tran = delProp.TransNode;

                        if (tran != null)
                        {
                            DestroyImmediate(tran.gameObject);
                        }
                    }
                }));
            }

            //set the List's sizeTODO

            return true;
        }

        //public Transform GetNewItem()
        //{
        //    //transform.GetSiblingIndex()
        //}

        public void UpdateValue(object data)
        {

        }

        public void Unbind()
        {
            base.UnbindBase();
            //disposables.Dispose();
            //throw new System.NotImplementedException();

            //TODO release the event OnAddItem ex.
        }
    }
}

