using IfeelgameFramework.Core.Logger;

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

        public void GenAndroidProj()
        {
            DebugEx.Log("AndroidStudio工程修改");
            
            //修改gradle文件
        }
    }
}
