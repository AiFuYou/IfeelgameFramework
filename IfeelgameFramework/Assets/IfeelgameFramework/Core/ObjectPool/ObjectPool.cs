/*
 * 使用此类及其衍生类时注意
 */

using System;
using System.Collections.Concurrent;

namespace IfeelgameFramework.Core.ObjectPool
{
    public sealed class ObjectPool<T> : IObjectPool
    {
        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _objectGenerator;
        private readonly Action<T> _objectRelease;
        private int _capacity;

        public ObjectPool(Func<T> objectGenerator, Action<T> objectRelease, int capacity = 5)
        {
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            _objectRelease = objectRelease ?? throw new ArgumentNullException(nameof(objectRelease));
            _capacity = capacity;
            _objects = new ConcurrentBag<T>();
        }

        public T Get() => _objects.TryTake(out T item) ? item : _objectGenerator();

        public void Put(T item)
        {
            if (Count < _capacity)
            {
                _objects.Add(item);
            }
            else
            {
                _objectRelease(item);
            }
        }

        public int Count => _objects.Count;

        public void SetCapacity(int capacity)
        {
            _capacity = capacity;
            
            if (Count > _capacity)
            {
                T _;
                while (Count > _capacity)
                {
                    _objects.TryTake(out _);
                    _objectRelease(_);
                }
            }
        }

        public void Clear()
        {
            T _;
            while (!_objects.IsEmpty) 
            {
                _objects.TryTake(out _);
                _objectRelease(_);
            }
        }
    }
}
