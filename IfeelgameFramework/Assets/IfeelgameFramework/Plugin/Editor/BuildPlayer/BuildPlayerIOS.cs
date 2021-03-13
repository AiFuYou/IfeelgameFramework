using IfeelgameFramework.Core.Logger;

namespace IfeelgameFramework.Plugin.Editor.BuildPlayer
{
    public class BuildPlayerIOS
    {
        private static BuildPlayerIOS _instance;
        private static readonly object _insLock = new object();
        /// <summary>
        /// 实例化LocalStorageManager单例，并创建存储文件夹
        /// </summary>
        public static BuildPlayerIOS Instance
        {
            get
            {
                lock (_insLock)
                {
                    if (_instance == null)
                    {
                        _instance = new BuildPlayerIOS();
                    }
                }

                return _instance;
            }
        }

        public void GenIOSProj()
        {
            DebugEx.Log("Xcode工程文件修改");
            
            //修改plist
            
            //修改project.pbxproj
        }
    }
}
