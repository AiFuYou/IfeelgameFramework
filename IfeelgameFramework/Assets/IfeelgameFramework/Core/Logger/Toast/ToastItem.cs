using System;
using System.Collections;
using IfeelgameFramework.Core.Coroutine;
using IfeelgameFramework.Core.Utils;
using UnityEngine;

namespace IfeelgameFramework.Core.Logger.Toast
{
    public class ToastItem : MonoBehaviour
    {
        private Action _removeAct;
        
        public void StartAnim(Action removeAct)
        {
            _removeAct = removeAct;
            StartCoroutine( Tools.WaitForSeconds(() =>
            {
                StartCoroutine(FadeOut());
            }, 2));
        }

        private IEnumerator FadeOut()
        {
            var cor = new CoroutineNumChange(1, 0, 60, opacity =>
            {
                GetComponent<CanvasGroup>().alpha = opacity;
            });
            yield return cor;

            _removeAct?.Invoke();
        }
    }
}
