using System;
using System.Collections.Generic;
using IfeelgameFramework.Core.Logger;
using UnityEngine;

namespace IfeelgameFramework.Core.Tasks
{
    [DefaultExecutionOrder(-100)]
    internal class MainThreadComponent : MonoBehaviour
    {
        private readonly List<ActionRunner> _listTaskRunner = new List<ActionRunner>();
        private readonly List<ActionRunner> _listTaskRunnerLateUpdate = new List<ActionRunner>();
        private readonly List<ActionRunner> _listTaskRunnerFixedUpdate = new List<ActionRunner>();
        
        public void AddTask(ActionRunner ar)
        {
            lock (_listTaskRunner)
            {
                _listTaskRunner.Add(ar);
            }
        } 
        
        public void AddTaskToLateUpdate(ActionRunner ar)
        {
            lock (_listTaskRunnerLateUpdate)
            {
                _listTaskRunnerLateUpdate.Add(ar);
            }
        }

        public void AddTaskToFixedUpdate(ActionRunner ar)
        {
            lock (_listTaskRunnerFixedUpdate)
            {
                _listTaskRunnerFixedUpdate.Add(ar);
            }
        }
    
        private void Update()
        {
            lock (_listTaskRunner)
            {
                while (_listTaskRunner.Count > 0)
                {
                    var ar = _listTaskRunner[0];
                    _listTaskRunner.RemoveAt(0);
                
                    try
                    {
                        ar.Run();
                    }
                    catch (Exception e)
                    {
                        DebugEx.Exception(e);
                    }
                }
            }
        }

        private void LateUpdate()
        {
            lock (_listTaskRunnerLateUpdate)
            {
                while (_listTaskRunnerLateUpdate.Count > 0)
                {
                    var ar = _listTaskRunnerLateUpdate[0];
                    _listTaskRunnerLateUpdate.RemoveAt(0);

                    try
                    {
                        ar.Run();
                    }
                    catch (Exception e)
                    {
                        DebugEx.Exception(e);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            lock (_listTaskRunnerFixedUpdate)
            {
                while (_listTaskRunnerFixedUpdate.Count > 0)
                {
                    var ar = _listTaskRunnerFixedUpdate[0];
                    _listTaskRunnerFixedUpdate.RemoveAt(0);

                    try
                    {
                        ar.Run();
                    }
                    catch (Exception e)
                    {
                        DebugEx.Exception(e);
                    }
                }
            }
        }
    }
}