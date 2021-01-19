using System.Collections.Generic;
using System.IO;
using IfeelgameFramework.Core.Logger;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace IfeelgameFramework.Plugin.Editor.BuildPlayer
{
    public class BuildPlayer : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private readonly string TAG = "BuildPlayer----------";
        public int callbackOrder => 0;

        /// <summary>
        /// Build前
        /// </summary>
        /// <param name="report"></param>
        public void OnPreprocessBuild(BuildReport report)
        {
            //通用操作
            DebugEx.Log(TAG, "OnPreprocessBuild", report.summary.platform, report.summary.outputPath);
            
            // Start listening for errors when build starts
            Application.logMessageReceived += OnBuildError;
            
            //根据LOG宏情况决定是否删除IfeelgameFramework里的Resources相关Debug资源
#if !LOG
            MoveAwayResources();
#endif
        }

        /// <summary>
        /// Build后
        /// </summary>
        /// <param name="report"></param>
        public void OnPostprocessBuild(BuildReport report)
        {
            //通用操作
            DebugEx.Log(TAG, "OnPostprocessBuild", report.summary.platform, report.summary.outputPath);

#if !LOG
            MoveBackResources();
#endif
        }
        
        /// <summary>
        /// Build失败回调
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stacktrace"></param>
        /// <param name="type"></param>
        private void OnBuildError(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Error)
            {
                // FAILED TO BUILD, STOP LISTENING FOR ERRORS
                Application.logMessageReceived -= OnBuildError;

#if !LOG
                MoveBackResources();
#endif
            }
        }

        #region MoveAwayAndBackResources

        private Dictionary<string, string> _oldNewPath = new Dictionary<string, string>();
        private readonly string _newPath = Directory.GetCurrentDirectory() + "/IfeelgameFrameworkTemp/";

        private void MoveAwayResources()
        {
            var ifeelgameFrameworkPath = Directory.GetCurrentDirectory() + "/Assets/IfeelgameFramework/";
            var dir = new DirectoryInfo(ifeelgameFrameworkPath);
        
            ListDir(dir);

            if (_oldNewPath.Count > 0)
            {
                foreach (var resourcesPath in _oldNewPath)
                {
                    Directory.Move(resourcesPath.Key, resourcesPath.Value);
                }
            }
        }
    
        private void ListDir(DirectoryInfo dir)
        {
            if (dir.ToString().Contains("Resources"))
            {
                if (!Directory.Exists(_newPath))
                {
                    Directory.CreateDirectory(_newPath);
                }
                _oldNewPath.Add(dir.ToString(), _newPath + _oldNewPath.Count + "/");
                return;
            }
        
            if (dir.GetDirectories("*").Length > 0)
            {
                foreach (var dirChild in dir.GetDirectories("*"))
                {
                    ListDir(dirChild);
                }
            }   
        }
        
        private void MoveBackResources()
        {
            foreach (var resourcesPath in _oldNewPath)
            {
                Directory.Move(resourcesPath.Value, resourcesPath.Key);
            }
        
            Directory.Delete(_newPath);
        }
        
        #endregion
    }
}
