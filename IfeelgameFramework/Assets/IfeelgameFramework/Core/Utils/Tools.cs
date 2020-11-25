using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using IfeelgameFramework.Core.Logger;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IfeelgameFramework.Core.Utils
{
    public static class Tools
    {
        #region 异步线程获取当前网络ip相关信息

        /// <summary>
        /// 异步线程获取当前网络ip，访问http://ip-api.com查看文档可以获取更多当前网络参数
        /// </summary>
        /// <param name="needWait">是否等待ip返回，默认等待</param>
        /// <returns>当前网络ip地址</returns>
        public static async Task<string> GetIpAsync(bool needWait = true)
        {
            if (string.IsNullOrEmpty(_ip))
            {
                if (needWait)
                {
                    await QueryIp(true);
                }
                else
                {
                    #pragma warning disable 4014
                    QueryIp(false);
                    #pragma warning restore 4014
                }
            }
            return _ip;
        }

        private static string _ip;
        private static HttpClient _clientIp;
        private static HttpResponseMessage _responseIp;
        private static bool _ipIsQuerying;
        private static Task _queryIpTask;
        private static async Task QueryIp(bool needWait)
        {
            if (_ipIsQuerying)
            {
                await _queryIpTask;
                return;
            }
            
            if (!string.IsNullOrEmpty(_ip)) return;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                DebugEx.Warning("no network");
                return;
            }
            
            _ipIsQuerying = true;
            _queryIpTask = Task.Run(async () =>
            {
                try
                {
                    if (_clientIp == null)
                    {
                        _clientIp = new HttpClient {Timeout = TimeSpan.FromSeconds(5)};
                    }

                    _responseIp =
                        await _clientIp.GetAsync("http://ip-api.com/json/?fields=status,message,query");
                    _responseIp.EnsureSuccessStatusCode(); //用来抛异常的
                    var responseBody = await _responseIp.Content.ReadAsStringAsync();
                    DebugEx.Log(responseBody);

                    if (_responseIp.IsSuccessStatusCode)
                    {
                        var result = JObject.Parse(responseBody);
                        if ((string) result["status"] == "success")
                        {
                            _ip = (string) result["query"];
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugEx.Error("Tools.GetIp\n" + e.Message);
                }
                finally
                {
                    _clientIp?.Dispose();
                    _clientIp = null;
                    
                    _responseIp?.Dispose();
                    _responseIp = null;
                    
                    _ipIsQuerying = false;
                }
            });

            if (needWait)
            {
                await _queryIpTask;
            }
        }
        
        #endregion

        #region Wait,Delay,仅限在协程里调用

        /// <summary>
        /// 等待一定时间后执行action
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="time">等待时间</param>
        /// <returns></returns>
        public static IEnumerator Run(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        /// <summary>
        /// 满足condition时执行action
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="action">方法</param>
        /// <returns></returns>
        public static IEnumerator WaitUntil(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);
            action();
        }

        /// <summary>
        /// 不满足condition时执行action
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="action">方法</param>
        /// <returns></returns>
        public static IEnumerator WaitWhile(Func<bool> condition, Action action)
        {
            yield return new WaitWhile(condition);
            action();
        }
        
        #endregion
    }
}
