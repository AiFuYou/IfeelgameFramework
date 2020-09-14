using System;
using System.Threading;
using UnityEngine;

namespace IfeelgameFramework.Core.Tasks
{
    /// <summary>
    /// runnable interface
    /// </summary>
    public interface IRunnable
    {
        void Run();
    }

    internal struct ActionRunner
    {
        internal System.Action action;
        internal IRunnable runnable;
        ManualResetEventSlim mres;

        internal ActionRunner(System.Action _action, IRunnable _runnable, bool _triggerEvent = false)
        {
            this.action = _action;
            this.runnable = _runnable;
            mres = _triggerEvent ? new ManualResetEventSlim() : null;
        }

        public void Run()
        {
            try
            {
                if (action != null)
                    action.Invoke();
                else
                    runnable.Run();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            mres?.Set();
        }

        public void Wait()
        {
            mres?.Wait();
        }
    }
}
