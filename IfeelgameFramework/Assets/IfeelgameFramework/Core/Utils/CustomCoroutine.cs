using System.Collections;

namespace IfeelgameFramework.Core.Utils
{
    /// <summary>
    /// 一个简单的协程实现，协程可以将复杂的任务拆分成小块执行，相应的执行时间也会变长，MoveNext每帧执行一次
    /// </summary>
    public class CustomCoroutine : IEnumerator
    {
        private bool IsDone => _index >= 500;
        private int _index;
    
        public bool MoveNext()
        {
            ++_index;
            return !IsDone;
        }

        public void Reset()
        {
            _index = 0;
        }

        public object Current => _index;
    }
}
