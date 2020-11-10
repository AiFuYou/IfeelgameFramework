using System;
using System.Net.Http;
using System.Threading.Tasks;
using IfeelgameFramework.Core.Logger;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace IfeelgameFramework.Core.Utils
{
    public class Tools
    {
        #region 异步线程获取当前网络ip
        
        /// <summary>
        /// 异步线程获取当前网络ip，访问http://ip-api.com查看文档可以获取更多当前网络参数
        /// </summary>
        /// <returns>当前网络ip地址</returns>
        public static string GetIp()
        {
            if (string.IsNullOrEmpty(_ip))
            {
                QueryIp();
            }

            return _ip;
        }

        private static string _ip;
        private static HttpClient _clientIp;
        private static HttpResponseMessage _responseIp;
        private static bool _ipIsQuerying;
        private static void QueryIp()
        {
            if (_ipIsQuerying) return;
            if (!string.IsNullOrEmpty(_ip)) return;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                DebugEx.Warning("no network");
                return;
            }
            
            _ipIsQuerying = true;
            Task.Run(async () =>
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
        }
        
        #endregion
    }
}
