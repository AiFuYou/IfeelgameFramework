using System.Collections.Generic;
using UnityEngine;

namespace IfeelgameFramework.Core.ObjectPool
{
    public class ObjectPool
    {
        private GameObject _prefabBase;
        private readonly List<GameObject> _list = new List<GameObject>();

        public void CreateObject(GameObject prefab, int count = 1, bool worldPositionStays = false)
        {
            
            
            _prefabBase = prefab;
            for (var i = 0; i < count; ++i)
            {
                var obj = Object.Instantiate(prefab, null, worldPositionStays);
                obj.gameObject.SetActive(false);
                _list.Add(obj);
            }
        }

        public GameObject Get()
        {
            if (_list.Count > 0)
            {
                var obj = _list[0];
                _list.RemoveAt(0);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                return Object.Instantiate(_prefabBase, null, false);
            }
        }

        public void Put(GameObject obj)
        {
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(false);
            _list.Add(obj);
        }

        public bool HaveObjectBase()
        {
            return _prefabBase != null;
        }

        public void Clear()
        {
            _prefabBase = null;
            _list.Clear();
        }
    }
}
