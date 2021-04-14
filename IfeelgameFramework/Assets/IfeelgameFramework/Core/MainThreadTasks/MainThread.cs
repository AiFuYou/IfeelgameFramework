using System;
using System.Threading;
using IfeelgameFramework.Core.Logger;
using UnityEngine;

namespace IfeelgameFramework.Core.MainThreadTasks
{
    public class MainThread
    {
        private static GameObject _mainThreadGameObject;
        private static Thread _mainThread;
        private static MainThreadComponent _taskExecutor;
    
        static MainThread()
        {
            var curThread = Thread.CurrentThread;
            if (curThread.ManagedThreadId > 1 || curThread.IsThreadPoolThread)
            {
                throw new Exception("Initialize the class in main thread please");
            }

            if (_mainThread == null)
            {
                _mainThread = curThread;
            }
        
            if (_mainThreadGameObject == null)
            {
                _mainThreadGameObject = new GameObject("MainThreadGameObject") {hideFlags = HideFlags.DontSave};
                _taskExecutor = _mainThreadGameObject.AddComponent<MainThreadComponent>();
            
                if (Application.isPlaying)
                {
                    UnityEngine.Object.DontDestroyOnLoad(_mainThreadGameObject);
                }
                
                DebugEx.Log("MainThread()");
            }
        }
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            DebugEx.Log("MainThread Initialize ok");
        }
    
        public static void RunTask(Action act)
        {
            if (IsMainThread())
            {
                act.Invoke();
            }
            else
            {
                var runner = new ActionRunner(act);
                _taskExecutor.AddTask(runner);
                runner.Wait();
            }
        }

        public static void AddTask(Action act)
        {
            _taskExecutor.AddTask(new ActionRunner(act));
        }

        public static void AddTaskToLateUpdate(Action act)
        {
            _taskExecutor.AddTaskToLateUpdate(new ActionRunner(act));
        }

        public static void AddTaskToFixedUpdate(Action act)
        {
            _taskExecutor.AddTaskToFixedUpdate(new ActionRunner(act));
        }

        private static bool IsMainThread()
        {
            return Thread.CurrentThread == _mainThread;
        }
    }
}