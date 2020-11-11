using System.Collections.Generic;
using UnityEngine;

namespace IfeelgameFramework.Core.ObjectPool
{
    public class GameObjectPool
    {
        private GameObject _prefabBase;
        private readonly Stack<GameObject> _gObjStack = new Stack<GameObject>();

        public GameObjectPool(GameObject gObj, int count = 0, bool worldPositionStays = false)
        {
            _prefabBase = gObj;
            if (count > 0)
            {
                for (var i = 0; i < count; ++i)
                {
                    var obj = Object.Instantiate(_prefabBase, null, worldPositionStays);
                    obj.gameObject.SetActive(false);
                    _gObjStack.Push(obj);
                }
            }
        }

        public GameObject Get()
        {
            if (Count > 0)
            {
                var obj = _gObjStack.Pop();
                obj.gameObject.SetActive(true);
                return obj;
            }

            return Object.Instantiate(_prefabBase, null, false);
        }

        public void Put(GameObject obj)
        {
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(false);
            _gObjStack.Push(obj);
        }

        public bool HaveObjectBase()
        {
            return _prefabBase != null;
        }

        public void Clear()
        {
            _prefabBase = null;
            while (Count > 0)
            {
                Object.Destroy(_gObjStack.Pop());
            }
            _gObjStack.Clear();
        }

        public int Count => _gObjStack.Count;
    }
}
