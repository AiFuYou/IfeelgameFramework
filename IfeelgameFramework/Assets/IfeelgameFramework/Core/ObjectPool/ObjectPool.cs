using System.Collections.Generic;

namespace IfeelgameFramework.Core.ObjectPool
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> _objectStack = new Stack<T>();

        public virtual T Get()
        {
            lock (_objectStack)
            {
                return Count > 0 ? _objectStack.Pop() : new T();
            }
        }

        public virtual void Put(T obj)
        {
            lock (_objectStack)
            {
                _objectStack.Push(obj);
            }
        }

        public virtual void Clear()
        {
            lock (_objectStack)
            {
                _objectStack.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (_objectStack)
                {
                    return _objectStack.Count;
                }
            }
        }
    }
}
