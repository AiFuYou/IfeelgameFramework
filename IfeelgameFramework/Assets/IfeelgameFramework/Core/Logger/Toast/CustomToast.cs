using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace IfeelgameFramework.Core.Logger.Toast
{
    public class CustomToast
    {
        private static CustomToast _instance;
        private static readonly object Padlock = new object();
        private static GameObject _toastGroupGameObject;
        
        public static CustomToast Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new CustomToast());
                }
            }
        }

        [Conditional("LOG")]
        public async void ShowToast(string str)
        {
            if (_toastGroupGameObject == null)
            {
                _toastGroupGameObject =
                    Object.Instantiate(await Addressables.LoadAssetAsync<GameObject>("ToastGroup").Task);
                _toastGroupGameObject.name = "ToastGroup";
                Object.DontDestroyOnLoad(_toastGroupGameObject);
            }
            
            _toastGroupGameObject.GetComponent<ToastGroup>().ShowToast(str);
        }
    }
}