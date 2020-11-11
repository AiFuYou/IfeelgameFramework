using System.Collections.Generic;
using IfeelgameFramework.Core.Logger;
using UnityEngine;

namespace IfeelgameFramework.Core.ObjectPool
{
    public class GameObjectPoolManager
    {
        private readonly Dictionary<string, GameObjectPool> _objectPoolDict = new Dictionary<string, GameObjectPool>();
        private static GameObjectPoolManager _instance;
        private static readonly Object LockPad = new Object();
        private readonly Dictionary<GameObject, string> _usingGameObjects = new Dictionary<GameObject, string>();

        public static GameObjectPoolManager Instance
        {
            get
            {
                lock (LockPad)
                {
                    return _instance ?? (_instance = new GameObjectPoolManager());
                }
            }
        }
        
        public void Add(string poolName, GameObject gObj, int count = 1)
        {
            if (!_objectPoolDict.ContainsKey(poolName))
            {
                var tmpPool = new GameObjectPool(gObj, count);
                _objectPoolDict.Add(poolName, tmpPool);
            }
            else
            {
                DebugEx.ErrorFormat("GameObjectPool {0} has been added, Don't add it again!", poolName);
            }
        }
        
        public GameObject Get(string poolName)
        {
            if (_objectPoolDict.ContainsKey(poolName))
            {
                var gObj = _objectPoolDict[poolName].Get();
                _usingGameObjects.Add(gObj, poolName);
                return gObj;
            }

            DebugEx.ErrorFormat("You must add {0} to GameObjectPool first", poolName);
            return null;
        }

        public void Put(string poolName, GameObject gObj)
        {
            if (_objectPoolDict.ContainsKey(poolName))
            {
                _objectPoolDict[poolName].Put(gObj);
            }
            else
            {
                DebugEx.ErrorFormat("ObjectPool {0} doesn't exist", poolName);
            }
        }

        public void Put(GameObject gObj)
        {
            if (_usingGameObjects.ContainsKey(gObj))
            {
                Put(_usingGameObjects[gObj], gObj);
                _usingGameObjects.Remove(gObj);
            }
            else
            {
                DebugEx.ErrorFormat("GameObject is not used", gObj.name);
            }
        }

        public bool HaveObject(string poolName)
        {
            return _objectPoolDict.ContainsKey(poolName) && _objectPoolDict[poolName].HaveObjectBase();
        }

        public void Clear(string poolName)
        {
            if (_objectPoolDict.ContainsKey(poolName))
            {
                _objectPoolDict[poolName].Clear();
            }
        }

        public void ClearAll()
        {
            foreach (var op in _objectPoolDict)
            {
                op.Value.Clear();
            }
        }

        public void DelGameObjectPool(string poolName)
        {
            if (_objectPoolDict.ContainsKey(poolName))
            {
                Clear(poolName);
                _objectPoolDict.Remove(poolName);
            }
        }
    }
}
