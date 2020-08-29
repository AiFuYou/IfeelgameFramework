using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace IfeelgameFramework.Core.Logger
{
    //Assert相关Debug有UNITY_ASSERTIONS，已由Unity自动处理，release包时不显示
    public class DebugEx
    {
        #region Constant

        private const string ColorLog = "green";
        private const string ColorError = "red";
        private const string ColorWarning = "orange";

        #endregion

        #region Log
        
        [Conditional("LOG")]
        public static void Log(params object[] message)
        {
            var s = "";
            foreach (var tmpS in message)
            {
                s += tmpS + "\t";
            }

            Debug.Log(AddColor(s, ColorLog));
        }

        [Conditional("LOG")]
        public static void Log(object message, Object context)
        {
            Debug.Log(AddColor(message, ColorLog), context);
        }

        public static void LogObject(object obj)
        {
            DebugEx.Log(JsonConvert.SerializeObject(obj));
        }
        
        [Conditional("LOG")]
        public static void LogFormat(string format, params object[] args)
        {
            Debug.LogFormat((string) AddColor(format, ColorLog), args);
        }

        [Conditional("LOG")]
        public static void LogFormat(Object context, string format, params object[] args)
        {
            Debug.LogFormat(context, (string) AddColor(format, ColorLog), args);
        }
        
        #endregion

        #region Error
        
        [Conditional("LOG")]
        public static void Error(params object[] message)
        {
            var s = "";
            foreach (var tmpS in message)
            {
                s += tmpS + "\t";
            }
            Debug.LogError(AddColor(s, ColorError));
        }

        [Conditional("LOG")]
        public static void Error(object message, Object context)
        {
            Debug.LogError(AddColor(message, ColorError), context);
        }
        
        [Conditional("LOG")]
        public static void ErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat((string) AddColor(format, ColorError), args);
        }

        [Conditional("LOG")]
        public static void ErrorFormat(Object context, string format, params object[] args)
        {
            Debug.LogErrorFormat(context, (string) AddColor(format, ColorError), args);
        }
        
        #endregion

        #region Exception

        [Conditional("LOG")]
        public static void Exception(Exception exception)
        {
            Debug.LogException(exception);
        }

        [Conditional("LOG")]
        public static void Exception(Exception exception, Object context)
        {
            Debug.LogException(exception, context);
        }

        #endregion

        #region Warning

        [Conditional("LOG")]
        public static void Warning(params object[] message)
        {
            var s = "";
            foreach (var tmpS in message)
            {
                s += tmpS + "\t";
            }
            Debug.LogWarning(AddColor(s, ColorWarning));
        }

        [Conditional("LOG")]
        public static void Warning(object message, Object context)
        {
            Debug.LogWarning(AddColor(message, ColorWarning), context);
        }
        
        [Conditional("LOG")]
        public static void WarningFormat(string format, params object[] args)
        {
            Debug.LogWarningFormat((string) AddColor(format, ColorWarning), args);
        }

        [Conditional("LOG")]
        public static void WarningFormat(Object context, string format, params object[] args)
        {
            Debug.LogWarningFormat(context, (string) AddColor(format, ColorWarning), args);
        }

        #endregion
        
        #region Other

        private static object AddColor(object logStr, string colorStr)
        {
            return "<color=" + colorStr + ">" + logStr + "</color>";
        }

        #endregion
    }
}
    
