
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
using UniRx;

namespace ZP.Lib
{
    /// <summary>
    /// ZUIR adio property item.support Enum Property and Link with List/RefList Property
    /// </summary>
    public class ZUIRadioPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        public Color UnselectedColor = Color.grey;
        public Color SelectedColor = Color.white;

        //use by Enum Property to define the Item Template
        //for List Property is define PropertyUIItemRes attribute. ex. [PropertyUIItemRes("DuduItems/CardLinkItemV", "Root")]
        public GameObject RadioItemTemplate;

       
        public Action<Transform> OnSelect;

        public IObservable<Transform> OnSelectObservable => 
            Observable.FromEvent<Action<Transform>, Transform>(h => a => h(a), h => OnSelect += h, h => OnSelect -= h);

        //private Image imageItem;
        private ImageResType resType = ImageResType.LocalRes;
        private string resPath = "";

       // private MultiDisposable disposables = new MultiDisposable();
        //for link list
        private Transform root;
        // private List<Transform> childs = new List<Transform>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        new public bool Bind(IZProperty property)
        {
            base.BindBase(property);
            if (IsLinkedProperty(property))
                return BindLink();


            if (ZPropertyMesh.IsEnum(property))

                return BindEnum();

            Debug.LogError("property value is not Enum or Property With ZpropertyList<String> Link");
            return false;
        }



        /// <summary>
        /// Binds the link.
        /// </summary>
        /// <returns><c>true</c>, if link was bound, <c>false</c> otherwise.</returns>
        private bool BindLink()
        {
            var link = (property as IZLinkable).LinkProperty;

            if (!ZPropertyMesh.IsPropertyListLike(link))
            {
                Debug.LogError("property value is Linkable but it is not a ZpropertyList<String> Link");
                return false;
            }

            //root = ZViewBuildTools.FindComponentInChildren<Transform>(this.transform, ZViewCommonObject.Root);

            //if (root == null)
            //{
            //    Debug.LogError("Can't find Root Transform to host the List " + property.PropertyID);
            //    return false;
            //}

            //bind the list like normal
            ZViewBuildTools.BindPropertyList(link, transform);

            //register button event
            root = ZViewBuildTools.FindComponentInChildren<Transform>(this.transform, ZViewCommonObject.Root);

            if (root == null)
                root = transform;

            //Bind the button's event handler
            int isel = (int)property.Value;
            AddListenerForLinkList();

            SetSelect();


            if (ZPropertyMesh.IsPropertyList(link))
            {
                (link as IZPropertyList).AddItemAsObservable().Subscribe(chprop =>
                {
                    ZViewBuildTools.BindProperty(chprop as IZProperty, transform);
                    //var node = (chprop as IZProperty).ViewItem as ZPropertyViewItemBehaviour;
                    SetUnSelectStatus((chprop as IZProperty).TransNode, false);

                    AddListenerForLinkList();
                });

                (link as IZPropertyList).OnRemoveItem += (chprop, index) =>
                {
                    Observable.NextFrame().Subscribe(_ => GameObject.DestroyImmediate(root.GetChild(index).gameObject));
                    //GameObject.DestroyImmediate( root.GetChild(index).gameObject);

                    AddListenerForLinkList();
                };
            }
            else if (ZPropertyMesh.IsPropertyRefList(link))
            {
                (link as IZPropertyRefList).AddItemAsObservable().Subscribe(chprop =>
                {
                    ZViewBuildTools.BindProperty(chprop as IZProperty, transform);
                    //var node = (chprop as IZProperty).ViewItem as ZPropertyViewItemBehaviour;
                    SetUnSelectStatus((chprop as IZProperty).TransNode, false);

                    AddListenerForLinkList();
                });

                (link as IZPropertyRefList).OnRemoveItem += (chprop, index) =>
                {
                    Observable.NextFrame().Subscribe(_ => GameObject.DestroyImmediate(root.GetChild(index).gameObject));
                    //GameObject.DestroyImmediate( root.GetChild(index).gameObject);

                    AddListenerForLinkList();
                };
            }


            return true;
        }

        /// <summary>
        /// Binds the enum.
        /// </summary>
        /// <returns><c>true</c>, if enum was bound, <c>false</c> otherwise.</returns>
        private bool BindEnum()
        {
            root = ZViewBuildTools.FindComponentInChildren<Transform>(this.transform, ZViewCommonObject.Root);

            if (root == null)
                root = transform;

            int iCount = 0;
            foreach (var suit in Enum.GetValues(property.GetDefineType()))
            {

                GameObject newItem = Instantiate(RadioItemTemplate, root) as GameObject;

                if (newItem == null)
                {
                    Debug.LogError("RadioItemTemplate is not set " + property.PropertyID);
                    return false;
                }

                var imageItem = newItem.GetComponentInChildren<Image>();
                //if (imageItem == null)
                //{
                //    Debug.LogError("Can't find Image Component " + property.PropertyID);
                //    //return false;
                //}

                var textCtrl = newItem.GetComponentInChildren<Text>();
                if (textCtrl != null)
                    textCtrl.text = suit.ToString(); //property.Value.ToString();

                var attr = property.AttributeNode.GetAttribute<PropertyImageResAttribute>();
                if (attr != null)
                {
                    resType = attr.ResType;
                    resPath = attr.Path;
                }

                var iCur = iCount;
                var btn = newItem.GetComponent<Button>();
                if (btn == null)
                {
                    Debug.LogError("RadioItemTemplate is Button set " + property.PropertyID);
                    return false;
                }

                btn.onClick.AddListener(() =>
                {
                    property.Value = iCur;
                });

                iCount++;

                if (imageItem != null)
                    StartCoroutine(LoadImage(imageItem, suit.ToString()));

                //set current's select
                // Console.WriteLine((int)suit + ":" + suit);

                BindEnumItem(newItem.transform);
            }

            SetSelect();

            return true;
        }

        new public void Unbind()
        {
            //TODO
            base.UnbindBase();
            disposables.Dispose();
        }

        private void adjustWdith()
        {
            var layout = root.GetComponent<HorizontalLayoutGroup>();

            var rtran = RadioItemTemplate.GetComponent<RectTransform>();

            var w = root.transform.childCount * (rtran.sizeDelta.x + layout.spacing);

        }
               
        private bool IsLinkedProperty(IZProperty prop)
        {
            if (!ZPropertyMesh.IsLinkable(prop))
                return false;
            var link = (prop as IZLinkable).LinkProperty;

            if (link == null)
            {
                return false;
            }

            return true;
            //var attr = link.AttributeNode.GetAttribute<PropertyLinkAttribute>();
            //return attr != null;
        }

        //TODO not very good
        private bool AddListenerForLinkList()
        {
            disposables.Dispose();

            for (int i = 0; i < root.childCount; i++)
            {
                var iCur = i;
                var btn = root.GetChild(i).GetComponentInChildren<Button>();
                if (btn == null)
                {
                    Debug.LogError("RadioItem has not Button set " + property.PropertyID);
                    return false;
                }
                //btn.onClick.RemoveAllListeners();

                //btn.onClick.AddListener(() =>
                //{
                //    property.Value = iCur;
                //});

                ZPropertyObservable.OnUnityEvent(btn.onClick).Subscribe(_=> {
                    property.Value = iCur;
                }).AddToMultiDisposable(disposables);
            }

            return true;
        }

        /// <summary>
        /// Sets the select enum.
        /// </summary>
        private void SetSelect()
        {
            int isel = (int)property.Value;

            for (int i = 0; i < root.childCount; i++)
            {
                //root.GetChild(i).GetComponent<Image>().color = isel == i ? Color.white : Color.grey;
                var propItem = root.GetChild(i).GetComponent<ZPropertyViewItemBehaviour>() as IZRadioViewItem;
                if (propItem != null)
                {
                    if (isel == i) propItem.OnSelected();
                    else propItem.OnUnselected();
                }
                else
                {
                    ZViewBuildTools.ChangeImageColor(root.GetChild(i), isel == i ? SelectedColor : UnselectedColor);
                }
            }

            if (OnSelect != null)
            {
                OnSelect(root.GetChild(isel));
            }

            ///var list = GetComponent<>

        }

        private  void SetUnSelectStatus(Transform tran, bool bSel)
        {
            var propItem = tran.GetComponent<ZPropertyViewItemBehaviour>() as IZRadioViewItem;
            if (propItem != null)
            {
                if (bSel) propItem.OnSelected();
                else propItem.OnUnselected();
            }
            else
            {
                ZViewBuildTools.ChangeImageColor(tran, bSel ? SelectedColor : UnselectedColor);
            }
        }

        /// <summary>
        /// Updates the value.
        /// </summary>
        /// <param name="data">Data.</param>
        new public void UpdateValue(object data)
        {
            // StartCoroutine(LoadImage(property.Value.ToString()));
            //if (IsLinkedProperty(property))
            //    SetSelectLink();

            //if (ZPropertyMgr.IsEnum(property))
                SetSelect();
        }


        /// <summary>
        /// Removes the item at index.
        /// </summary>
        /// <param name="index">Index.</param>
        public void RemoveItemAt(int index)
        {
            DestroyImmediate(root.GetChild(index).gameObject);
            AddListenerForLinkList();
        }

        private IEnumerator LoadImage(Image imageItem, string str)
        {
            if (resType == ImageResType.LocalRes)
            {
                var res = Resources.Load<Sprite>(this.resPath + str);
                if (res != null)
                {
                    imageItem.sprite = res;
                }
                else
                {
                    Debug.LogWarning("There is No Image Res in local!!" + this.resPath + str);

                }
            }
            else if (resType == ImageResType.WebRes)
            {
                //WWW www = new WWW(this.resPath + property.Value.ToString());
                UnityWebRequest www = UnityWebRequest.Get(this.resPath + str);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogWarning(www.error);
                }
                else
                {
                    Texture2D tex2d = ((DownloadHandlerTexture)www.downloadHandler).texture;

                    Sprite m_sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
                    imageItem.sprite = m_sprite;
                }

            }

            yield return null;
        }

        private void BindEnumItem(Transform item)
        {
            var propItems = item.GetComponents<IZPropertyViewItem>();
            if (propItems != null)
            {

                //prop.ViewItem = propItem;
                foreach (var p in propItems)
                {
                    if (!p.Bind(null)) // property is Enum
                    {
                        //delete the bind
                        //prop.ViewItem = null;
                    }
                }

                return;
            }
        }

        //void SetItemSelected(Transform tran)
        //{
        //    tran.GetComponent<Image>().color = isel == i ? Color.white : Color.grey;
        //}

        /// <summary>
        /// Ons the click on link list.
        /// </summary>
        /// <param name="prop">Property.</param>
        /// <param name="tran">Tran.</param>
        /// <param name="index">Index.</param>
        //private void OnClickOnLinkList(IZProperty prop, Transform tran, int index)
        //{
        //    property.Value = index;
        //}

        /// <summary>
        /// Sets the select link.
        /// </summary>
        //void SetSelectLink()
        //{
        //    int isel = (int)property.Value;

        //    for (int i = 0; i < childs.Count; i++)
        //    {
        //        childs[i].GetComponent<Image>().color = isel == i ? Color.white : Color.grey;
        //    }

        //    if (OnSelect != null)
        //    {
        //        OnSelect(childs[isel]);
        //    }
        //}

#endif
    }

}

