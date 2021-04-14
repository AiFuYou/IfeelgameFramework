using IfeelgameFramework.Core.Logger;

namespace IfeelgameFramework.Plugin.Editor.BuildPlayer
{
    public class BuildPlayerIOS
    {
        private BuildPlayerIOS(){}
        private static BuildPlayerIOS _instance;
        private static readonly object _insLock = new object();
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

        public void GenProj()
        {
            DebugEx.Log("Xcode工程文件修改");
            
            //修改plist
            
            //修改project.pbxproj
        }
        
        public void Package()
        {
            
        }

        private void PackageDebug()
        {
            
        }

        private void PackageRelease()
        {
            
        }
    }
}
