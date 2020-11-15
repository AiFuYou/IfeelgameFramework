namespace IfeelgameFramework.Core.ObjectPool
{
    public interface IObjectPool
    {
        void Clear();
        void SetCapacity(int capacity);
    }
}
