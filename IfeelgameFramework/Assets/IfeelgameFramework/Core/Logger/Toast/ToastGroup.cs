using System.Collections.Generic;
using IfeelgameFramework.Core.ObjectPool;
using UnityEngine;
using UnityEngine.UI;

namespace IfeelgameFramework.Core.Logger.Toast
{
    public class ToastGroup : MonoBehaviour
    {
        public GameObject toast;
        private ObjectPool<GameObject> _toastPool;
        private List<GameObject> _toastList;
        private const string ToastObjectPool = "ToastObjectPool";
    
        public void ShowToast(string str)
        {
            if (_toastList == null)
            {
                _toastList = new List<GameObject>();
            }

            if (_toastPool == null)
            {
                _toastPool = ObjectPoolManager.Instance.Create(ToastObjectPool, () => Instantiate(toast), Destroy);
            }
        
            CheckToastList();
        
            var toastObj = _toastPool.Get();
            _toastList.Add(toastObj);
            toastObj.transform.SetParent(transform, false);
            toastObj.transform.GetComponent<CanvasGroup>().alpha = 1;
            toastObj.transform.localPosition = new Vector2(0, -700);
            toastObj.transform.Find("text").GetComponent<Text>().text = str;
            toastObj.GetComponent<ToastItem>().StartAnim(RemoveToast);
        }

        private void CheckToastList()
        {
            if (_toastList.Count <= 0) return;
        
            foreach (var t in _toastList)
            {
                var oriY = t.transform.localPosition.y;
                t.transform.localPosition = new Vector2(0, oriY + 110);
            }
        }

        private void RemoveToast()
        {
            
        }
    }
}
