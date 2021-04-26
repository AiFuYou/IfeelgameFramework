using UnityEngine;

namespace IfeelgameFramework.Core.Utils
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                        
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
    }
}
