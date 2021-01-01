using System;
using System.Threading;
using IfeelgameFramework.Core.Logger;

namespace IfeelgameFramework.Core.MainThreadTasks
{
    internal class ActionRunner
    {
        private readonly ManualResetEventSlim _manualResetEventSlim = new ManualResetEventSlim(false);
        private Action _act;

        public ActionRunner(Action act)
        {
            _act = act;
        }

        public void Run()
        {
            try
            {
                _act?.Invoke();
            }
            catch (Exception e)
            {
                DebugEx.Exception(e);
            }
        
            _manualResetEventSlim?.Set();
        }

        public void Wait()
        {
            _manualResetEventSlim?.Wait();
        }
    }
}