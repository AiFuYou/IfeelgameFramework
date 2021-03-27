using IfeelgameFramework.Core.Logger;
using IfeelgameFramework.Core.Storage;
using UnityEditor;
using UnityEngine;

namespace IfeelgameFramework.Plugin.Editor.Menu
{
    public class MenuGroup : MonoBehaviour
    {
        [MenuItem("IfeelgameFramework/Clear All UserData")] //保存后就会在unity菜单栏中出现MyMenu的菜单栏
        static void ClearAllUserData() //和二级目录Do Something,点击它就会执行DoSomething()方法
        {
            PlayerPrefs.DeleteAll();
            LocalStorageManager.Instance.ClearAllData();
            
            DebugEx.Log("Clear All UserData Successful");
        }
    }
}
