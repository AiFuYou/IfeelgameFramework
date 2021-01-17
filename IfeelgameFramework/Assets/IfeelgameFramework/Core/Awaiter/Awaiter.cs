using System;

namespace IfeelgameFramework.Core.Awaiter
{
    public sealed class Awaiter : IAwaiter, IAwaitable<Awaiter>
    {
        private Action _continuation;
        
        public void OnCompleted(Action continuation)
        {
            if (!IsCompleted)
            {
                _continuation = continuation;
            }
            else
            {
                continuation?.Invoke();
            }
        }

        public bool IsCompleted { get; private set; }
        public void GetResult() {}

        public Awaiter GetAwaiter()
        {
            return this;
        }

        public void Done()
        {
            IsCompleted = true;
                     
            var continuation = _continuation;
            continuation?.Invoke();
            _continuation = null;
        }
    }
}