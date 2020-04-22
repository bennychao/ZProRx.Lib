
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if ZP_UNITY_CLIENT
using UnityEngine.UI;
#endif
namespace ZP.Lib
{
    /// <summary>
    /// Image res type.
    /// </summary>
    public enum ImageResType
    {
        LocalRes,
        WebRes
    }




    /// <summary>
    /// ZUII mage property item.
    /// </summary>
    public class ZUIImagePropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {

#if ZP_UNITY_CLIENT

        private Image imageItem;
        private ImageResType resType = ImageResType.LocalRes;
        private string resPath = "";
        // Use this for initialization
        void Start()
        {

        }

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

            if (gameObject.activeSelf)
                StartCoroutine(LoadImage());
            return true;
        }

        new public void Unbind()
        {
            base.UnbindBase();

            if (imageItem.sprite)
                Resources.UnloadAsset(imageItem.sprite);
        }

        new public void UpdateValue(object data)
        {
            if (gameObject.activeSelf)
                StartCoroutine(LoadImage());
        }

        private IEnumerator LoadImage()
        {
            if (resType == ImageResType.LocalRes)
            {
                var res = Resources.Load<Sprite>(this.resPath + property.Value.ToString());
                if (res != null)
                {
                    imageItem.sprite = res;
                }
                else
                {
                    Debug.LogWarning("There is No Image Res in local!! " + this.resPath + property.Value.ToString());

                }
            }
            else if (resType == ImageResType.WebRes)
            {
                //WWW www = new WWW(this.resPath + property.Value.ToString());
                UnityWebRequest www = UnityWebRequest.Get(this.resPath + property.Value.ToString());
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


        // Update is called once per frame
        void Update()
        {

        }
#endif
    }
}


