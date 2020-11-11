/*
 * 此类慎用，
 */

using System.Collections.Generic;

namespace IfeelgameFramework.Core.ObjectPool
{
    /// <summary>
    /// Dictionary
    /// key:string, value:string
    /// 对象池
    /// </summary>
    public class DictStrStrPool : ObjectPool<Dictionary<string, string>>
    {
        public override Dictionary<string, string> Get()
        {
            var obj = base.Get();
            obj.Clear();
            return obj;
        }

        public override void Put(Dictionary<string, string> obj)
        {
            obj.Clear();
            base.Put(obj);
        }
    }

    /// <summary>
    /// Dictionary
    /// key:string, value:int
    /// 对象池
    /// </summary>
    public class DictStrIntPool : ObjectPool<Dictionary<string, int>>
    {
        public override Dictionary<string, int> Get()
        {
            var obj = base.Get();
            obj.Clear();
            return obj;
        }

        public override void Put(Dictionary<string, int> obj)
        {
            obj.Clear();
            base.Put(obj);
        }
    }
}
