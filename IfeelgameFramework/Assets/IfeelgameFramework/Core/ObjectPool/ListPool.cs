using System.Collections.Generic;

namespace IfeelgameFramework.Core.ObjectPool
{
    /// <summary>
    /// List
    /// string
    /// 对象池
    /// </summary>
    public class ListStrPool : ObjectPool<List<string>>
    {
        public override List<string> Get()
        {
            var obj = base.Get();
            obj.Clear();
            return obj;
        }

        public override void Put(List<string> obj)
        {
            obj.Clear();
            base.Put(obj);
        }
    }

    /// <summary>
    /// List
    /// int
    /// 对象池
    /// </summary>
    public class ListIntPool : ObjectPool<List<int>>
    {
        public override List<int> Get()
        {
            var obj = base.Get();
            obj.Clear();
            return base.Get();
        }

        public override void Put(List<int> obj)
        {
            obj.Clear();
            base.Put(obj);
        }
    }
}
