using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IfeelgameFramework.Core.Utils
{
    public class NativeBridge
    {
        #region Instance
        
#if !UNITY_EDITOR && UNITY_ANDROID
        private AndroidJavaClass _pluginAndroid = null;
#endif
        
        private static NativeBridge _instance;
        private static readonly Object InstanceLock = new Object();

        private NativeBridge()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            _pluginAndroid = new AndroidJavaClass("com.ifeel.game.framework.NativeUnityPlugin");
#endif
        }
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

        #endregion

        #region RegionName

#if !UNITY_EDITOR && UNITY_IOS 
        [DllImport("__Internal")]
        private static extern string mGetRegionName();
#endif
        
        public string GetRegionName()
        {
            var regionName = RegionInfo.CurrentRegion.Name;
#if !UNITY_EDITOR && UNITY_ANDROID
            regionName = _pluginAndroid.CallStatic<string>("GetRegionName");
#elif !UNITY_EDITOR && UNITY_IOS
            regionName = mGetRegionName();
#endif
            return regionName;
        }

        #endregion

        #region Language

#if !UNITY_EDITOR && UNITY_IOS 
        [DllImport("__Internal")]
        private static extern string mGetLanguage();
#endif

        public string GetLanguage()
        {
            var language = Application.systemLanguage.ToString(); 
#if !UNITY_EDITOR && UNITY_ANDROID
            language = _pluginAndroid.CallStatic<string>("GetLanguage");
#elif !UNITY_EDITOR && UNITY_IOS
            language = mGetLanguage();
#endif
            return language;
        }

        #endregion

        #region UniqueId

        public string GetAndroidId()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            return _pluginAndroid.CallStatic<string>("GetAndroidId");
#else
            return null;
#endif
        }

#if !UNITY_EDITOR && UNITY_IOS 
        [DllImport("__Internal")]
        private static extern string mGetUUID();
#endif
        
        public string GetUUID()
        {
#if !UNITY_EDITOR && UNITY_IOS
            return mGetUUID();
#else
            return null;
#endif
        }
        
        #endregion
    }
}