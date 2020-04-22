// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

//[Version:0.81]
using UnityEngine;

namespace ZP.Lib.Common
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _Instance;

        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<T>();
                }
                return _Instance;
            }
        }

        /// <summary>
        /// release the singleton
        /// </summary>
        public static void ReleaseInstance()
        {
            _Instance = null;
        }
    }


}