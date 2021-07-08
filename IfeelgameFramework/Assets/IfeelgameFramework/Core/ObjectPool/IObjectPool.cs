namespace IfeelgameFramework.Core.ObjectPool
{
    public interface IObjectPool<T> : IObjectPool
    {
        void Put(T t);
        T Get();
    }
    
    public interface IObjectPool
    {
        void Clear();
        void SetCapacity(int capacity);
    }
}
