using System;
using System.Collections.Generic;

namespace IfeelgameFramework.Core.Messenger
{
    public class Messenger
    {
        private Dictionary<string, Delegate> _eventDict = new Dictionary<string, Delegate>();
        
        #region Instance

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

        #endregion

        #region AddListener

        public void AddListener(string eventName, Action act)
        {
            OnAddingListener(eventName, act);
            _eventDict[eventName] = (Action) _eventDict[eventName] + act;
        }

        public void AddListener<T>(string eventName, Action<T> act)
        {
            OnAddingListener(eventName, act);
            _eventDict[eventName] = (Action<T>) _eventDict[eventName] + act;
        }
        
        public void AddListener<T, TU>(string eventName, Action<T, TU> act)
        {
            OnAddingListener(eventName, act);
            _eventDict[eventName] = (Action<T, TU>) _eventDict[eventName] + act;
        }
        
        public void AddListener<T, TU, TV>(string eventName, Action<T, TU, TV> act)
        {
            OnAddingListener(eventName, act);
            _eventDict[eventName] = (Action<T, TU, TV>) _eventDict[eventName] + act;
        }
        
        public void AddListener<T, TU, TV, TW>(string eventName, Action<T, TU, TV, TW> act)
        {
            OnAddingListener(eventName, act);
            _eventDict[eventName] = (Action<T, TU, TV, TW>) _eventDict[eventName] + act;
        }

        private void OnAddingListener(string eventName, Delegate d)
        {
            if (!_eventDict.ContainsKey(eventName))
            {
                _eventDict.Add(eventName, null);
            }

            var tmpD = _eventDict[eventName];
            if (tmpD != null && tmpD.GetType() != d.GetType())
            {
                throw new Exception(
                    $"Attempting to add listener with inconsistent signature for event type {eventName}. Current listeners have type {tmpD.GetType().Name} and listener being added has type {d.GetType().Name}");
            }
        }
        
        #endregion

        #region RemoveListener

        public void RemoveListener(string eventName, Action act)
        {
            OnRemovingListener(eventName, act);
            if (_eventDict.ContainsKey(eventName))
            {
                _eventDict[eventName] = (Action) _eventDict[eventName] - act;    
            }
            OnRemovedListener(eventName);
        }
        
        public void RemoveListener<T>(string eventName, Action<T> act)
        {
            OnRemovingListener(eventName, act);
            if (_eventDict.ContainsKey(eventName))
            {
                _eventDict[eventName] = (Action<T>) _eventDict[eventName] - act;
            }
            OnRemovedListener(eventName);
        }
        
        public void RemoveListener<T, TU>(string eventName, Action<T, TU> act)
        {
            OnRemovingListener(eventName, act);
            if (_eventDict.ContainsKey(eventName))
            {
                _eventDict[eventName] = (Action<T, TU>) _eventDict[eventName] - act;
            }
            OnRemovedListener(eventName);
        }
        
        public void RemoveListener<T, TU, TV>(string eventName, Action<T, TU, TV> act)
        {
            OnRemovingListener(eventName, act);
            if (_eventDict.ContainsKey(eventName))
            {
                _eventDict[eventName] = (Action<T, TU, TV>) _eventDict[eventName] - act;
            }
            OnRemovedListener(eventName);
        }
        
        public void RemoveListener<T, TU, TV, TW>(string eventName, Action<T, TU, TV, TW> act)
        {
            OnRemovingListener(eventName, act);
            if (_eventDict.ContainsKey(eventName))
            {
                _eventDict[eventName] = (Action<T, TU, TV, TW>) _eventDict[eventName] - act;
            }
            OnRemovedListener(eventName);
        }

        private void OnRemovingListener(string eventName, Delegate d)
        {
            if (_eventDict.ContainsKey(eventName))
            {
                Delegate tmpD = _eventDict[eventName];
                if (tmpD == null)
                {
                    throw new Exception(
                        $"Attempting to remove listener with for event type \"{eventName}\" but current listener is null.");
                }
                else
                {
                    if (tmpD.GetType() != d.GetType())
                    {
                        throw new Exception(
                            $"Attempting to remove listener with inconsistent signature for event type {eventName}. Current listeners have type {tmpD.GetType().Name} and listener being removed has type {d.GetType().Name}");
                    }
                }
            }
            else
            {
                throw new Exception(
                    $"Attempting to remove listener for type \"{eventName}\" but Messenger doesn't know about this event type.");
            }
        }

        private void OnRemovedListener(string eventName)
        {
            if (_eventDict.ContainsKey(eventName) && _eventDict[eventName] == null)
            {
                _eventDict.Remove(eventName);
            }
        }

        #endregion

        #region BroadCast

        public void BroadCast(string eventName)
        {
            OnBroadCasting(eventName);
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action;
                act?.Invoke();
            }
        }
        
        public void BroadCast<T>(string eventName, T t)
        {
            OnBroadCasting(eventName);
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action<T>;
                act?.Invoke(t);
            }
        }
        
        public void BroadCast<T, TU>(string eventName, T t, TU tu)
        {
            OnBroadCasting(eventName);
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action<T, TU>;
                act?.Invoke(t, tu);
            }
        }
        
        public void BroadCast<T, TU, TV>(string eventName, T t, TU tu, TV tv)
        {
            OnBroadCasting(eventName);
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action<T, TU, TV>;
                act?.Invoke(t, tu, tv);
            }
        }
        
        public void BroadCast<T, TU, TV, TW>(string eventName, T t, TU tu, TV tv, TW tw)
        {
            OnBroadCasting(eventName);
            if (_eventDict.TryGetValue(eventName, out var d))
            {
                var act = d as Action<T, TU, TV, TW>;
                act?.Invoke(t, tu, tv, tw);
            }
        }

        private void OnBroadCasting(string eventName)
        {
		    if (!_eventDict.ContainsKey(eventName))
		    {
		        throw new Exception($"Broadcasting message \"{eventName}\" but no listener found.");
		    }
        }
        
        #endregion
    }
}