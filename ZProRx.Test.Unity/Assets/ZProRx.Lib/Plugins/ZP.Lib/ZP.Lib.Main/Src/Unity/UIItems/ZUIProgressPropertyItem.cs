using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ZP.Lib.Unity
{
	//support float Property
    public class ZUIProgressPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
    private Image progressPointer;

    /// <summary>
    /// Updates the value.
    /// </summary>
    /// <param name="data">Data.</param>
    void IZPropertyViewItem.UpdateValue(object data)
    {
         progressPointer.fillAmount = Mathf.Clamp((property as IZProperty<float>).Value, 0, 1);
    }

    /// <summary>
    /// Bind the specified property.
    /// </summary>
    /// <param name="property">Property.</param>
    bool IZPropertyViewItem.Bind(IZProperty property)
    {

        base.BindBase(property);
        progressPointer = ZViewBuildTools.FindComponentInChildren<Image>(this.transform, "Progress");
                       
        if (progressPointer == null)
        {
            Debug.LogError("Can't find Data Component " + property.PropertyID);
            return false;
        }

        progressPointer.fillAmount = Mathf.Clamp((property as IZProperty<float>).Value, 0, 1);

        return true;
    }

    // new public void Unbind()
    // {
    //     //do nothing
    // }
#endif
}
}
