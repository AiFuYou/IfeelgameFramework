using IfeelgameFramework.Core.Logger;
using UnityEditor;
using UnityEngine;

namespace IfeelgameFramework.Plugin.Editor.BuildPlayer
{
    public static class BuildPlayerHelper
    {
        public static string GetPath(string fileName)
        {
            var path = AssetDatabase.FindAssets(fileName);
            if (path.Length > 1)
            {
                Debug.LogError("有同名文件"+fileName+"获取路径失败");
                return null;
            }
        
            //将字符串中得脚本名字和后缀统统去除掉
            return AssetDatabase.GUIDToAssetPath(path[0]);
        }
        
        public static void Log(params object[] message)
        {
            var s = "";
            foreach (var tmpS in message)
            {
                s += tmpS + "\t";
            }

            Debug.Log(DebugEx.AddColor(s, DebugEx.ColorLog));
        }
        
        public static void Error(params object[] message)
        {
            var s = "";
            foreach (var tmpS in message)
            {
                s += tmpS + "\t";
            }
            Debug.LogError(DebugEx.AddColor(s, DebugEx.ColorError));
        }
    }
}