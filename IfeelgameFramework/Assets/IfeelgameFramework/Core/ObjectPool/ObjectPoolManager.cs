using System;
using System.Collections.Generic;
using IfeelgameFramework.Core.Logger;

namespace IfeelgameFramework.Core.ObjectPool
{
    public class ObjectPoolManager
    {
        private readonly Dictionary<string, IObjectPool> _objectPoolDict = new Dictionary<string, IObjectPool>();
        
        private ObjectPoolManager(){}
        private static ObjectPoolManager _instance;
        private static readonly object LockPad = new object();

        public static ObjectPoolManager Instance
        {
            get
            {
                lock (LockPad)
                {
                    return _instance ??= new ObjectPoolManager();
                }
            }
        }

        public ObjectPool<T> Create<T>(string poolName, Func<T> objectGenerator, Action<T> objectRelease)
        {
            if (!_objectPoolDict.ContainsKey(poolName))
            {
                _objectPoolDict.Add(poolName, new ObjectPool<T>(objectGenerator, objectRelease));
            }
            else
            {
                DebugEx.ErrorFormat("ObjectPool {0} has been added, Don't add it again!", poolName);            
            }
            return (ObjectPool<T>) _objectPoolDict[poolName];
        }

        public T Get<T>(string poolName)
        {
            if (_objectPoolDict.ContainsKey(poolName))
            {
                return ((ObjectPool<T>) _objectPoolDict[poolName]).Get();
            }
            
            DebugEx.ErrorFormat("You must add {0} to ObjectPoolManager first", poolName);
            return default;
        }

        public ObjectPool<T> GetPool<T>(string poolName)
        {
            if (_objectPoolDict.ContainsKey(poolName))
            {
                return (ObjectPool <T>) _objectPoolDict[poolName];
            }

            return null;
        }

        public void Put<T>(string poolName, T obj)
        {
            if (_objectPoolDict.ContainsKey(poolName))
            {
                ((ObjectPool<T>) _objectPoolDict[poolName]).Put(obj);
            }
            else
            {
                DebugEx.ErrorFormat("You must add {0} to ObjectPoolManager first", poolName);
            }
        }

        public void SetPoolCapacity(string poolName, int capacity)
        {
            if (_objectPoolDict.ContainsKey(poolName))
            {
                _objectPoolDict[poolName].SetCapacity(capacity);
            }
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
            foreach (var item in _objectPoolDict)
            {
                item.Value.Clear();
            }
            _objectPoolDict.Clear();
        }
    }
}
