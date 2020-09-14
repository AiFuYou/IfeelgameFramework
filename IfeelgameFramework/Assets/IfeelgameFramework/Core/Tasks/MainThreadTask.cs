using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IfeelgameFramework.Core.Tasks
{
    [DefaultExecutionOrder(-100)]
    internal class MainThreadExecutorComponent: MonoBehaviour
    {
        static MainThreadExecutorComponent _instance = null;
        List<ActionRunner> _taskList = new List<ActionRunner>();
        List<ActionRunner> _lateTaskList = new List<ActionRunner>();

        private void Start()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(this);
        }

        private void Update()
        {
            lock(_taskList)
            {
                while (_taskList.Count > 0)
                {
                    ActionRunner runner = _taskList[0];
                    _taskList.RemoveAt(0);

                    try
                    {
                        runner.Run();
                    }
                    catch(Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            lock (_lateTaskList)
            {
                while (_lateTaskList.Count > 0)
                {
                    ActionRunner runner = _lateTaskList[0];
                    _lateTaskList.RemoveAt(0);

                    try
                    {
                        runner.Run();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
            }
        }

        public void AddTask(ActionRunner runner, bool ignoreIfExists)
        {
            lock(_taskList)
            {
                if (ignoreIfExists)
                {
                    for (int i = 0; i < _taskList.Count; i++)
                        if (_taskList[i].action == runner.action && _taskList[i].runnable == runner.runnable)
                            return;
                }
                _taskList.Add(runner);
            }
        }

        public void AddTaskToLateUpdate(ActionRunner runner, bool ignoreIfExists)
        {
            lock (_lateTaskList)
            {
                if (ignoreIfExists)
                {
                    for (int i = 0; i < _lateTaskList.Count; i++)
                        if (_lateTaskList[i].action == runner.action && _lateTaskList[i].runnable == runner.runnable)
                            return;
                }
                _lateTaskList.Add(runner);
            }
        }
    }

    /// <summary>
    /// schedule a task on main thread
    /// </summary>
    public static class MainThreadTask
	{
        static GameObject _mainThreadRunnerObject;
        static MainThreadExecutorComponent _executor;

        private static Thread _mainThread;

        static MainThreadTask()
        {
            Thread currentThread = Thread.CurrentThread;
            if (currentThread.ManagedThreadId > 1 || currentThread.IsThreadPoolThread)
                throw new Exception("Initialize the class on the main thread, please!");
            _mainThread = currentThread;

            if (_mainThreadRunnerObject == null)
            {
                _mainThreadRunnerObject = new GameObject("MainThreadTaskRunnerGameObject")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                Object.DontDestroyOnLoad(_mainThreadRunnerObject);
            }
            
            _executor = _mainThreadRunnerObject.AddComponent<MainThreadExecutorComponent>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            if (_mainThread == null)
                Debug.Log("Init MainThreadTask");   // this won't happen
        }

        /// <summary>
        /// check if current thread is main thread
        /// </summary>
        /// <returns>true if we are in main thread</returns>
        public static bool IsMainThread() { return Thread.CurrentThread == _mainThread; }

        /// <summary>
        /// run a task on main thread and wait for return
        /// </summary>
        /// <param name="action">action</param>
        public static void RunTask(System.Action action)
        {
            if (IsMainThread())
                action.Invoke();
            else
            {
                ActionRunner runner = new ActionRunner(action, null, true);
                _executor.AddTask(runner, false);
                runner.Wait();
            }            
        }

        /// <summary>
        /// add a task to run on main thread (Update)
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="ignoreIfExists">ignore it if it's already in task list</param>
        public static void AddTask(System.Action action, bool ignoreIfExists = false)
        {
            _executor.AddTask(new ActionRunner(action, null), ignoreIfExists);
        }

        /// <summary>
        /// add a task to run on main thread (LateUpdate)
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="ignoreIfExists">ignore it if it's already in task list</param>
        public static void AddTaskToLateUpdate(System.Action action, bool ignoreIfExists = false)
        {
            _executor.AddTaskToLateUpdate(new ActionRunner(action, null), ignoreIfExists);
        }

        /// <summary>
        /// Run a task on main thread and wait for return
        /// </summary>
        /// <param name="runnable">Runnable object</param>
        public static void RunTask(IRunnable runnable)
        {
            if (IsMainThread())
                runnable.Run();
            else
            {
                ActionRunner runner = new ActionRunner(null, runnable, true);
                _executor.AddTask(runner, false);
                runner.Wait();
            }
        }

        /// <summary>
        /// Add a task on main thread (Update)
        /// </summary>
        /// <param name="runnable">Runnable object</param>
        /// <param name="ignoreIfExists">ignore it if it's already in task list</param>
        public static void AddTask(IRunnable runnable, bool ignoreIfExists = false)
        {
            _executor.AddTask(new ActionRunner(null, runnable), ignoreIfExists);
        }

        /// <summary>
        /// Add a task on main thread (LateUpdate)
        /// </summary>
        /// <param name="runnable">Runnable object</param>
        /// <param name="ignoreIfExists">ignore it if it's already in task list</param>
        public static void AddTaskToLateUpdate(IRunnable runnable, bool ignoreIfExists = false)
        {
            _executor.AddTaskToLateUpdate(new ActionRunner(null, runnable), ignoreIfExists);
        }
    }
}
