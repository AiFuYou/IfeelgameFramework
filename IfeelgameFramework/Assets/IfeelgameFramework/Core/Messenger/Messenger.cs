using System;
using System.Collections.Generic;

namespace IfeelgameFramework.Core.Messenger
{
    public class Messenger
    {
        private Messenger(){}
        
        private static Messenger _instance;
        private static readonly object _insLock = new object();
        
        public static Messenger Instance
        {
            get
            {
                lock (_insLock)
                {
                    return _instance ?? (_instance = new Messenger());
                }
            }
        }

        private Dictionary<string, Delegate> _eventDict = new Dictionary<string, Delegate>();

        #region AddListener

        public void AddListener(string eventName, Action act)
        {
            _eventDict[eventName] = (Action) _eventDict[eventName] + act;
        }

        public void AddListener<T>(string eventName, Action<T> act)
        {
            _eventDict[eventName] = (Action<T>) _eventDict[eventName] + act;
        }
        
        public void AddListener<T, TU>(string eventName, Action<T, TU> act)
        {
            _eventDict[eventName] = (Action<T, TU>) _eventDict[eventName] + act;
        }
        
        public void AddListener<T, TU, TV>(string eventName, Action<T, TU, TV> act)
        {
            _eventDict[eventName] = (Action<T, TU, TV>) _eventDict[eventName] + act;
        }
        
        public void AddListener<T, TU, TV, TW>(string eventName, Action<T, TU, TV, TW> act)
        {
            _eventDict[eventName] = (Action<T, TU, TV, TW>) _eventDict[eventName] + act;
        }
        
        #endregion

        #region RemoveListener

        public void RemoveListener(string eventName, Action act)
        {
            _eventDict[eventName] = (Action) _eventDict[eventName] - act;
        }
        
        public void RemoveListener<T>(string eventName, Action<T> act)
        {
            _eventDict[eventName] = (Action<T>) _eventDict[eventName] - act;
        }
        
        public void RemoveListener<T, TU>(string eventName, Action<T, TU> act)
        {
            _eventDict[eventName] = (Action<T, TU>) _eventDict[eventName] - act;
        }
        
        public void RemoveListener<T, TU, TV>(string eventName, Action<T, TU, TV> act)
        {
            _eventDict[eventName] = (Action<T, TU, TV>) _eventDict[eventName] - act;
        }
        
        public void RemoveListener<T, TU, TV, TW>(string eventName, Action<T, TU, TV, TW> act)
        {
            _eventDict[eventName] = (Action<T, TU, TV, TW>) _eventDict[eventName] - act;
        }

        #endregion

        #region BroadCast

        public void BroadCast(string eventName)
        {
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action;
                act?.Invoke();
            }
        }
        
        public void BroadCast<T>(string eventName, T t)
        {
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action<T>;
                act?.Invoke(t);
            }
        }
        
        public void BroadCast<T, TU>(string eventName, T t, TU tu)
        {
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action<T, TU>;
                act?.Invoke(t, tu);
            }
        }
        
        public void BroadCast<T, TU, TV>(string eventName, T t, TU tu, TV tv)
        {
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action<T, TU, TV>;
                act?.Invoke(t, tu, tv);
            }
        }
        
        public void BroadCast<T, TU, TV, TW>(string eventName, T t, TU tu, TV tv, TW tw)
        {
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action<T, TU, TV, TW>;
                act?.Invoke(t, tu, tv, tw);
            }
        }
        
        #endregion
    }
}