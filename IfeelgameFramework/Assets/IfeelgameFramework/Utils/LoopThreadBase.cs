using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace IfeelgameFramework.Utils
{
    public class LoopThread
    {
        public int ThreadId;
    
        private bool _isExiting;
        public Handler Handler;
        private Loop _loop;
        private Thread _thread;
    
        public void Start()
        {
            _isExiting = false;
            _loop = new Loop();
            Handler = new Handler(_loop);
            _thread = new Thread(Run);
            _thread.Start();
        }

        public void Exit()
        {
            _isExiting = true;
            _thread?.Abort();
        }
    
        void Run()
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            while (!_isExiting)
            {
                _loop.Prepare();
                while (true)
                {
                    try
                    {
                        if (!_loop.Execute())
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Loop failed, but continue");
                        Debug.LogException(e);
                    }
                }
            }
        }
    }

    public class Loop
    {
        private readonly List<Action> _actionQueue;
        private readonly ManualResetEvent _manualResetEvent;

        public Loop()
        {
            _manualResetEvent = new ManualResetEvent(false);
            _actionQueue = new List<Action>();
        }

        public void AddAction(Action action)
        {
            lock (_actionQueue)
            {
                _actionQueue.Add(action);
            }
            _manualResetEvent.Set();
        }

        public void Prepare()
        {
            _manualResetEvent.WaitOne();
            _manualResetEvent.Reset();
        }

        public bool Execute()
        {
            Action callback;
            lock (_actionQueue)
            {
                if (_actionQueue.Count == 0)
                {
                    return false;
                }
                callback = _actionQueue[0];
                _actionQueue.RemoveAt(0);
            }

            try
            {
                callback();
            }
            catch (Exception e)
            {
                Debug.LogError("Execute callback failed");
                Debug.LogException(e);
            }

            return true;
        }
    }

    public class Handler
    {
        private readonly Loop _loop;

        public Handler(Loop loop)
        {
            _loop = loop;
        }

        public void Post(Action act)
        {
            _loop.AddAction(act);
        }
    }
}