using System.Diagnostics;
using System.IO;
using IfeelgameFramework.Core.Logger;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build.Reporting;

namespace IfeelgameFramework.Plugin.Editor.BuildPlayer
{
    public class BuildPlayerAndroid
    {
        private static BuildPlayerAndroid _instance;
        private static readonly object _insLock = new object();
        /// <summary>
        /// 实例化LocalStorageManager单例，并创建存储文件夹
        /// </summary>
        public static BuildPlayerAndroid Instance
        {
            get
            {
                lock (_insLock)
                {
                    if (_instance == null)
                    {
                        _instance = new BuildPlayerAndroid();
                    }
                }

                return _instance;
            }
        }

        public void GenProj()
        {
            BuildPlayerHelper.Log("AndroidStudio工程修改");
            
            //修改gradle文件
        }

        public void Package(BuildReport report)
        {
            var shScriptPath = BuildPlayerHelper.GetPath("build_android");
            var projPath = report.summary.outputPath;
            var gradlePath = AndroidExternalToolsSettings.gradlePath;
            
            //寻找gradle-launcher-*.jar文件
            var files = Directory.GetFiles(gradlePath, "gradle-launcher-*.jar", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                gradlePath = files[0];
            }
            else
            {
                BuildPlayerHelper.Error("not find file gradle-launcher-*.jar");
                return;
            }
            
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = $"{shScriptPath} {projPath} {gradlePath} {(EditorUserBuildSettings.development ? "debug" : "release")}"
            };

            var p = Process.Start(psi);
            if (p != null)
            {
                var strOutput = p.StandardOutput.ReadToEnd();
                var errorOutput = p.StandardError.ReadToEnd();
                p.WaitForExit();
                BuildPlayerHelper.Log(strOutput);

                if (!string.IsNullOrEmpty(errorOutput))
                {
                    BuildPlayerHelper.Error(errorOutput);    
                }
            }
            else
            {
                BuildPlayerHelper.Error("build_android.sh executed failed");
            }
        }
    }
}
