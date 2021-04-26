using System.Collections.Generic;
using System.IO;
using UnityEditor;
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
            BuildPlayerHelper.Log(TAG, "OnPreprocessBuild", report.summary.platform, report.summary.outputPath);
            
            // Start listening for errors when build starts
            Application.logMessageReceived += OnBuildError;

            if (report.summary.platform == BuildTarget.Android)
            { 
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
                EditorUserBuildSettings.buildAppBundle = false;
                // EditorUserBuildSettings.androidCreateSymbolsZip = true;//如果设置了exportAsGoogleAndroidProject为true，则此设置不生效

                if (EditorUserBuildSettings.development)
                {
                    EditorUserBuildSettings.connectProfiler = true;
                    EditorUserBuildSettings.buildWithDeepProfilingSupport = true;
                    EditorUserBuildSettings.allowDebugging = true;
                }
            }
            
            //设置LOG宏定义
            if (EditorUserBuildSettings.development)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(report.summary.platformGroup, "LOG");    
            }
            else
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(report.summary.platformGroup, "");
            }
        }

        /// <summary>
        /// Build后
        /// </summary>
        /// <param name="report"></param>
        public void OnPostprocessBuild(BuildReport report)
        {
            //通用操作
            BuildPlayerHelper.Log(TAG, "OnPostprocessBuild", report.summary.platform, report.summary.outputPath);

            //build出工程之后对工程文件做相应的修改
#if UNITY_ANDROID
            BuildPlayerAndroid.Instance.GenProj();
            BuildPlayerAndroid.Instance.Package(report);
#elif UNITY_IOS
            BuildPlayerIOS.Instance.GenProj();
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
            }
        }

    }
}
