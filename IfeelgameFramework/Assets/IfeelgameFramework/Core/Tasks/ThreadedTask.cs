using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace IfeelgameFramework.Core.Tasks
{
    internal class TaskThread
    {
        List<ActionRunner> queuedActions = new List<ActionRunner>();
        Thread thread = null;

        public int Pending
        {
            get
            {
                lock (queuedActions)
                {
                    int ret = queuedActions.Count;
                    if (thread != null)
                        ret++;
                    return ret;
                }
            }
        }

        public void AddTask(System.Action action)
        {
            lock (queuedActions)
            {
                queuedActions.Add(new ActionRunner(action, null));
                if (thread == null)
                {
                    thread = new System.Threading.Thread(DoJobs);
                    thread.Start();
                }
            }
        }

        public void AddTask(IRunnable runnable)
        {
            lock (queuedActions)
            {
                queuedActions.Add(new ActionRunner(null, runnable));
                if (thread == null)
                {
                    thread = new System.Threading.Thread(DoJobs);
                    thread.Start();
                }
            }
        }

        public void DoJobs()
        {
            ActionRunner runner;
            while (true)
            {
                lock (queuedActions)
                {
                    if (queuedActions.Count == 0)
                    {
                        thread = null;
                        return;
                    }

                    runner = queuedActions[0];
                    queuedActions.RemoveAt(0);
                }

                runner.Run();
            }
        }

        public IEnumerator WaitFor()
        {
            while (Pending > 0)
            {
                yield return null;
            }
        }

        internal bool FindAction(System.Action action)
        {
            lock(queuedActions)
            {
                for (int i = 0; i < queuedActions.Count; i++)
                {
                    if (queuedActions[i].action == action)
                        return true;
                }
            }
            return false;
        }

        internal bool FindRunnable(IRunnable runnable)
        {
            lock (queuedActions)
            {
                for (int i = 0; i < queuedActions.Count; i++)
                {
                    if (queuedActions[i].runnable == runnable)
                        return true;
                }
            }
            return false;
        }

        internal bool RemoveAction(System.Action action)
        {
            bool ret = false;
            lock (queuedActions)
            {
                for (int i = queuedActions.Count - 1; i >= 0; i--)
                {
                    if (queuedActions[i].action == action)
                    {
                        ret = true;
                        queuedActions.RemoveAt(i);
                    }
                }
            }
            return ret;
        }

        internal bool RemoveRunnable(IRunnable runnable)
        {
            bool ret = false;
            lock (queuedActions)
            {
                for (int i = queuedActions.Count - 1; i >= 0; i--)
                {
                    if (queuedActions[i].runnable == runnable)
                    {
                        ret = true;
                        queuedActions.RemoveAt(i);
                    }
                }
            }
            return ret;
        }
    }

    /// <summary>
    /// Execute tasks on thread pool
    /// </summary>
    public class ThreadedTaskPool
    {
        int maxPoolSize = 1;
        List<TaskThread> threads = new List<TaskThread>();

        /// <summary>
        /// default thread pool
        /// </summary>
        public static ThreadedTaskPool Default = new ThreadedTaskPool();

        /// <summary>
        /// create thread pool
        /// </summary>
        /// <param name="size">max thread count</param>
        public ThreadedTaskPool(int size = 1)
        {
            SetPoolSize(size);
        }

        /// <summary>
        /// set max thread count
        /// </summary>
        /// <param name="size">max thread count</param>
        public void SetPoolSize(int size)
        {
            if (size < 1)
                size = 1;
            maxPoolSize = size;
        }

        void RemoveIdleThreads()
        {
            while (threads.Count > maxPoolSize)
            {
                bool removed = false;
                for (int i = threads.Count - 1; i >= 0; i--)
                {
                    if (threads[i].Pending == 0)
                    {
                        threads.RemoveAt(i);
                        removed = true;
                        if (threads.Count <= maxPoolSize)
                            return;
                    }
                }
                if (!removed)
                    break;
            }
        }

        /// <summary>
        /// add task to pool
        /// </summary>
        /// <param name="action">action</param>
        /// <param name="ignoreIfExists">ignore it if it's already in pool</param>
        public void AddTask(System.Action action, bool ignoreIfExists = false)
        {
            lock (threads)
            {
                RemoveIdleThreads();
                if (ignoreIfExists)
                {
                    for (int i = 0; i < threads.Count; i++)
                    {
                        if (threads[i].FindAction(action))
                            return;
                    }
                }

                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].Pending == 0)
                    {
                        threads[i].AddTask(action);
                        return;
                    }
                }

                if (threads.Count < maxPoolSize)
                {
                    TaskThread tthread = new TaskThread();
                    threads.Add(tthread);
                    tthread.AddTask(action);
                    return;
                }

                int max = int.MaxValue;
                int index = -1;
                for (int i = 0; i < threads.Count; i++)
                {
                    int n = threads[i].Pending;
                    if (n < max)
                    {
                        max = n;
                        index = i;
                    }
                }

                threads[index].AddTask(action);
            }
        }

        /// <summary>
        /// add task to pool
        /// </summary>
        /// <param name="runnable">runnable</param>
        /// <param name="ignoreIfExists">ignore it if it's already in pool</param>
        public void AddTask(IRunnable runnable, bool ignoreIfExists = false)
        {
            lock(threads)
            {
                RemoveIdleThreads();
                if (ignoreIfExists)
                {
                    for (int i = 0; i < threads.Count; i++)
                    {
                        if (threads[i].FindRunnable(runnable))
                            return;
                    }
                }

                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].Pending == 0)
                    {
                        threads[i].AddTask(runnable);
                        return;
                    }
                }

                if (threads.Count < maxPoolSize)
                {
                    TaskThread tthread = new TaskThread();
                    threads.Add(tthread);
                    tthread.AddTask(runnable);
                    return;
                }

                int max = int.MaxValue;
                int index = -1;
                for (int i = 0; i < threads.Count; i++)
                {
                    int n = threads[i].Pending;
                    if (n < max)
                    {
                        max = n;
                        index = i;
                    }
                }

                threads[index].AddTask(runnable);
            }

        }

        /// <summary>
        /// remove task from pool
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>true if removed successfully</returns>
        public bool RemoveTask(System.Action action)
        {
            bool ret = false;
            lock(threads)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].RemoveAction(action))
                        ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// remove task from pool
        /// </summary>
        /// <param name="runnable">runnable</param>
        /// <returns>true if removed successfully</returns>
        public bool RemoveTask(IRunnable runnable)
        {
            bool ret = false;
            lock (threads)
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    if (threads[i].RemoveRunnable(runnable))
                        ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Get pending task count
        /// </summary>
        /// <returns></returns>
        public int PendingTasks()
        {
            int count = 0;
            for (int i = 0; i < threads.Count; i++)
                count += threads[i].Pending;
            return count;
        }

        /// <summary>
        /// run task on an extra thread
        /// </summary>
        /// <param name="action">action</param>
        public static void RunTaskOnExtraThread(System.Action action)
        {
            TaskThread tthread = new TaskThread();
            tthread.AddTask(action);
        }

        /// <summary>
        /// run task on an extra thread
        /// </summary>
        /// <param name="runnable">runnable</param>
        public static void RunTaskOnExtraThread(IRunnable runnable)
        {
            TaskThread tthread = new TaskThread();
            tthread.AddTask(runnable);
        }

    }
}
