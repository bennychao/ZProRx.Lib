#if ZP_UNITY_CLIENT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ZP.Lib
{
    //[InitializeOnLoad]
    //public class ZViewItemHierachyCallBack
    //{
    //    // 层级窗口项回调
    //    private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;

    //    private static Texture2D hierarchyIcon;
    //    private static Texture2D HierarchyIcon
    //    {
    //        get
    //        {
    //            if (ZViewItemHierachyCallBack.hierarchyIcon == null)
    //            {
    //                ZViewItemHierachyCallBack.hierarchyIcon = (Texture2D)Resources.Load("Test/User/TestUser2");
    //            }
    //            return ZViewItemHierachyCallBack.hierarchyIcon;
    //        }
    //    }

    //    private static Texture2D hierarchyEventIcon;
    //    private static Texture2D HierarchyEventIcon
    //    {
    //        get
    //        {
    //            if (ZViewItemHierachyCallBack.hierarchyEventIcon == null)
    //            {
    //                ZViewItemHierachyCallBack.hierarchyEventIcon = (Texture2D)Resources.Load("Test/User/TestUser1");
    //            }
    //            return ZViewItemHierachyCallBack.hierarchyEventIcon;
    //        }
    //    }

    //    /// <summary>
    //    /// 静态构造
    //    /// </summary>
    //    static ZViewItemHierachyCallBack()
    //    {
    //        //Debug.Log("ZViewItemHierachyCallBack");
    //        ZViewItemHierachyCallBack.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(ZViewItemHierachyCallBack.DrawHierarchyIcon);
    //        EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(
    //            EditorApplication.hierarchyWindowItemOnGUI,
    //            ZViewItemHierachyCallBack.hiearchyItemCallback);

    //        //EditorApplication.update += Update;
    //    }

    //    // 绘制icon方法
    //    private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
    //    {
    //        if (!Application.isPlaying)
    //            return;

    //        //Debug.Log("DrawHierarchyIcon");
    //        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;


    //        if (gameObject == null)
    //            return;

    //        // 设置icon的位置与尺寸（Hierarchy窗口的左上角是起点）
    //        Rect rect = new Rect(selectionRect.x + selectionRect.width - 2f, selectionRect.y, 16f, 16f);
    //        // 画icon

    //        //IZPropertyViewItem item = null;

    //        if (gameObject.GetComponent<IZPropertyViewItem>() != null)
    //        {
    //            GUI.DrawTexture(rect, ZViewItemHierachyCallBack.HierarchyEventIcon);
    //        }
    //        else if (gameObject.GetComponent<IZEventItem>() != null)
    //        {
    //            GUI.DrawTexture(rect, ZViewItemHierachyCallBack.HierarchyIcon);
    //        }
    //    }

    //}

}
#endif
