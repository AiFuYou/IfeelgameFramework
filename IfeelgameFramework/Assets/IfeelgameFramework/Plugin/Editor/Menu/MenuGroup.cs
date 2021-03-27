using IfeelgameFramework.Core.Logger;
using IfeelgameFramework.Core.Storage;
using UnityEditor;
using UnityEngine;

namespace IfeelgameFramework.Plugin.Editor.Menu
{
    public class MenuGroup : MonoBehaviour
    {
        [MenuItem("IfeelgameFramework/DeleteAllData")] //保存后就会在unity菜单栏中出现MyMenu的菜单栏
        static void DeleteAllData() //和二级目录Do Something,点击它就会执行DoSomething()方法
        {
            PlayerPrefs.DeleteAll();
            LocalStorageManager.Instance.DeleteAllData();
            
            DebugEx.Log("DeleteAllData Successful");
        }
    }
}
