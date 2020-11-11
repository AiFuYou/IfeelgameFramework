using System.Collections.Generic;
using System.Threading.Tasks;
using IfeelgameFramework.Core.Logger;
using IfeelgameFramework.Core.ObjectPool;
using IfeelgameFramework.Core.Sound;
using IfeelgameFramework.Core.Storage;
using IfeelgameFramework.Core.Tasks;
using IfeelgameFramework.Core.Utils;
using UnityEngine;

public class Demo : MonoBehaviour
{
    private const string TAG = "Demo";
    
    // Start is called before the first frame update
    void Start()
    {
        // SoundManagerTest();
        GameObjectPoolTest();
        // WriteRecordTest();
        // GetIpTest();
        // LocalStorageTest();
        // TaskTest();
        
        // DebugEx.Log(TAG, SystemInfo.operatingSystem);
        // DebugEx.Log(TAG, SystemInfo.operatingSystemFamily);
    }

    #region MainThreadTaskTest

    private void TaskTest()
    {
        Task.Run(() =>
        {
            MainThread.AddTaskToFixedUpdate(() =>
            {
                DebugEx.Log("MainThread", "fixedUpdateTask");
            });
        });
        
        Task.Run(() =>
        {
            MainThread.AddTask(() =>
            {
                DebugEx.Log("MainThread", "addTask");
            });
        });
        
        Task.Run(() =>
        {
            MainThread.AddTaskToLateUpdate(() =>
            {
                DebugEx.Log("MainThread", "lateUpdateTask");
            });
        });
        
        Task.Run(() =>
        {
            MainThread.RunTask(() =>
            {
                DebugEx.Log("MainThread", "runTask");
            });
        });
    }
    
    #endregion

    #region LocalStorageTest

    private class MyClass
    {
        public int a = 1;
        public List<Dictionary<string, int>> e = new List<Dictionary<string, int>>();
        public Dictionary<string, object> d = new Dictionary<string, object>();
    }
    
    private void LocalStorageTest()
    {
        var ls = LocalStorageManager.Instance.GetLocalStorage("test");
        // var ls = LocalStorageManager.Instance;

        ls.SetValue("short", (short)1, true);
        ls.SetValue("int", 1, true, true);
        ls.SetValue("float", 1.1f, true);
        ls.SetValue("long", (long)1, true, true);
        ls.SetValue("bool", false, true);
        ls.SetValue("string", "1231231", true, true);
        ls.SetValue("double", 22.2, true);

        var myClass = new MyClass();
        myClass.a = 3;

        var dic = new Dictionary<string, int> {{"s", 1}};
        myClass.e.Add(dic);
        myClass.d.Add("a", false);
        
        ls.SetValue("test", myClass, true);

        var b1 = ls.GetValue<short>("short");
        var b2 = ls.GetValue<int>("int");
        var b3 = ls.GetValue<float>("float");
        var b4 = ls.GetValue<long>("long");
        var b5 = ls.GetValue<bool>("bool");
        var b6 = ls.GetValue<double>("double");
        var b7 = ls.GetValue<Dictionary<string, object>>("object");
        var b8 = ls.GetValue<string>("string");
        var b9 = ls.GetValue<MyClass>("test");

        DebugEx.Log("111");
    }
    
    #endregion

    #region GetIpTest

    private async void GetIpTest()
    {
        DebugEx.Log(Tools.GetIp());
        await Task.Delay(1000);
        DebugEx.Log(Tools.GetIp());
    }
    
    #endregion

    #region GameObjectPoolTest

    private void GameObjectPoolTest()
    {
        var gObj = new GameObject("12312");
        GameObjectPoolManager.Instance.Add("test", gObj);

        for (var i = 0; i < 10; i++)
        {
            var go = GameObjectPoolManager.Instance.Get("test");
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector2.zero;
            go.name = i.ToString();
            GameObjectPoolManager.Instance.Put(go);
        }

        var dictStrStrPool = new DictStrStrPool();
        for (var i = 0; i < 10; i++)
        {
            var a = dictStrStrPool.Get();
            a.Add("test", "test");
            dictStrStrPool.Put(a);

            var b = dictStrStrPool.Get();
            dictStrStrPool.Put(b);
        }
        
        DebugEx.Log(dictStrStrPool.Count);
        dictStrStrPool.Clear();
        DebugEx.Log(dictStrStrPool.Count);
    }
    
    #endregion

    #region WriteRecordTest

    private async void WriteRecordTest()
    {
        await Task.Delay(3000);
        gameObject.AddComponent<FileRecordQueueTest>();
    }
    
    #endregion

    #region SoundManagerTest

    private void SoundManagerTest()
    {
        SoundManager.InitData();
        SoundManager.PlayMusic("bg");
    }
    
    #endregion


    // Update is called once per frame
    void Update()
    {
        
    }
}
