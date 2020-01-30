using System;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine;
using ZP.Lib;

namespace ZP.Lib.Unity.ViewCore
{
    public static class ZViewBuildToolsEx
    {
        public static IObservable<Unit> OnEnableAsObservable(this Transform transform)
        {
            var vRoot = transform.GetComponent<ZPropertyViewRootBehaviour>();
            if (vRoot == null)
                throw new Exception("Subscribe Error Transform with OnEnable Observable");

            return vRoot.OnEnableObservable;
        }

        public static IObservable<Unit> OnDisableAsObservable(this Transform transform)
        {
            var vRoot = transform.GetComponent<ZPropertyViewRootBehaviour>();
            if (vRoot == null)
                throw new Exception("Subscribe Error Transform with OnEnable Observable");

            return vRoot.OnDisableObservable;
        }
    }
}
