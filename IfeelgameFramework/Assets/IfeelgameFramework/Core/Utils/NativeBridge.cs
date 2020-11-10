﻿using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IfeelgameFramework.Core.Utils
{
    public class NativeBridge
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        private AndroidJavaClass _pluginAndroid = null;
#endif
        private static NativeBridge _instance;
        private static readonly Object InstanceLock = new Object();

        private NativeBridge() {}
        public static NativeBridge Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    return _instance ?? (_instance = new NativeBridge());
                }     
            }
        }
        
#if !UNITY_EDITOR && UNITY_IOS 
        [DllImport("__Internal")]
        private static extern string mGetRegionName();
#endif
        
        public string GetRegionName()
        {
            var regionName = RegionInfo.CurrentRegion.Name;
#if !UNITY_EDITOR && UNITY_ANDROID
            if (_pluginAndroid == null)
            {
                _pluginAndroid = new AndroidJavaClass("com.ifeel.game.framework.NativeUnityPlugin");
            }

            regionName = _pluginAndroid.CallStatic<string>("GetRegionName");
#elif !UNITY_EDITOR && UNITY_IOS
            regionName = mGetRegionName();
#endif
            return regionName;
        }
    }
}