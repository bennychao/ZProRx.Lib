
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib
{


    public class ZUIImageStatusPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        private Image imageItem;
        private ImageResType resType = ImageResType.LocalRes;
        private string resPath = "";

        new public bool Bind(IZProperty property)
        {
            base.BindBase(property);
            imageItem = GetComponent<Image>();

            if (imageItem == null)
            {
                imageItem = ZViewBuildTools.FindComponentInChildren<Image>(this.transform, ZViewCommonObject.Image);
            }

            if (imageItem == null)
            {
                Debug.LogError("Can't find Image Component " + property.PropertyID);
                return false;
            }


            var attr = property.AttributeNode.GetAttribute<PropertyImageResAttribute>();
            if (attr != null)
            {
                resType = attr.ResType;
                resPath = attr.Path;
            }

            //if (!property.GetDefineType().i && property.Value != null)
            //if ()
                StartCoroutine(LoadImage(property.Value.ToString()));
            return true;
        }

       new public void Unbind()
        {
            //do nothing
            base.UnbindBase();

            StopAllCoroutines();
        }

        new public void UpdateValue(object data)
        {
            StartCoroutine(LoadImage(property.Value.ToString()));
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private IEnumerator LoadImage(string str)
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
                    Debug.LogWarning("There is No Image Res in local!!");

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

#endif
    }

}

