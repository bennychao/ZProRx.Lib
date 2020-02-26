using UnityEngine;
using System.Collections;
using ZP.Lib;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using ZP.Lib.CoreEx;
using UniRx;

namespace ZP.Lib.Unity
{
    //now only support enum property
    public class ZUIDropdownPropertyItem : ZUIPropertyItemBehaviour, IZPropertyViewItem
    {
#if ZP_UNITY_CLIENT
        Dropdown dropdown = null;
#endif
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
#if ZP_UNITY_CLIENT
            dropdown = transform.GetComponentInChildren<Dropdown>();

            if (dropdown == null)
            {
                Debug.LogError("Can't find DropDown Component " + property.PropertyID + " transform is  " + transform.name);
                return false;
            }
#endif
            BindEnum();
            return true;
        }

        public void UpdateValue(object data)
        {
#if ZP_UNITY_CLIENT
            dropdown.value = (int)data;
#endif
        }

        private bool BindEnum()
        {
            //root = ZViewBuildTools.FindComponentInChildren<Transform>(this.transform, ZViewCommonObject.Root);

//if (root == null)
//root = transform;
#if ZP_UNITY_CLIENT
            dropdown.ClearOptions();

            List<Dropdown.OptionData> odList = new List<Dropdown.OptionData>();

            foreach (var suit in Enum.GetValues(property.GetDefineType()))
            {
                var od = new Dropdown.OptionData();
                od.text = suit.ToString();

                //get enumStr
                odList.Add(od);
            }
            dropdown.AddOptions(odList);

            dropdown.onValueChanged.ToObservable().Subscribe(v =>
            {
                property.Value = v;
            }).AddToMultiDisposable(disposables);
#endif
            return true;
        }

    }

}

