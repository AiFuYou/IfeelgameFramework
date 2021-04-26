using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using IfeelgameFramework.Core.Awaiter;
using IfeelgameFramework.Core.Coroutine;
using IfeelgameFramework.Core.Logger;
using IfeelgameFramework.Core.Logger.Toast;
using IfeelgameFramework.Core.MainThreadTasks;
using IfeelgameFramework.Core.Messenger;
using IfeelgameFramework.Core.ObjectPool;
using IfeelgameFramework.Core.Utils;
using UnityEngine;

public class Demo : MonoBehaviour
{
    private const string TAG = "Demo";

    // Start is called before the first frame update
    void Awake()
    {
        // SoundManagerTest();
        // GameObjectPoolTest();
        // GetIpTest();
        // LocalStorageTest();
        // TaskTest();
        // MessengerTest();
        // NativeBridgeTest();
        // AwaiterTest();
        // StartCoroutine(CoroutineTest());
        // ToastTest();
    }

    #region ToastTest

    private async void ToastTest()
    {
        var count = 1;
        while (Application.isPlaying)
        {
            CustomToast.Instance.ShowToast("Test CustomToast " + count);
            ++count;
            await Task.Delay(1000);
        }
    }

    #endregion

    #region CoroutineTest

    private IEnumerator CoroutineTest()
    {
        DebugEx.Log("1");
        var cc = new CoroutineNumChange(0, 10, 60, f => DebugEx.Log(f));
        yield return cc;
        DebugEx.Log("2");
    }

    #endregion

    #region AwaiterTest

    private async void AwaiterTest()
    {
        var awaiterTest = new Awaiter();
        StartCoroutine(Tools.WaitForSeconds(() => { awaiterTest.Done(); }, 3));

        DebugEx.Log(TAG, "AwaiterTest", "awaiterTest 等待中");
        await awaiterTest;
        DebugEx.Log(TAG, "AwaiterTest", "awaiterTest Done");
    }

    #endregion

    #region MessengerTest

    private void MessengerTest()
    {
        Messenger.Instance.AddListener("test", MessengerFunc);
        Messenger.Instance.BroadCast("test");
        Messenger.Instance.RemoveListener("test", MessengerFunc);
        Messenger.Instance.BroadCast("test");
    }

    private void MessengerFunc()
    {
        DebugEx.Log("BroadCast");
    }

    #endregion

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

    private void LocalStorageTest()
    {
        transform.Find("LocalStorageManagerTest").gameObject.SetActive(true);
        GetComponentInChildren<LocalStorageManagerTest>().InitTest();
    }
    
    #endregion

    #region GetIpTest

    private async void GetIpTest()
    {
        //传入false，不等待结果返回
        DebugEx.Log(TAG, "不等待", await Tools.GetIpAsync(false));

        //默认传入true，等待网络结果返回
        var ip2 = await Tools.GetIpAsync();
        DebugEx.Log(TAG, "等待", ip2);
        
        DebugEx.Log(TAG, "不等待，但结果在上一步已返回", await Tools.GetIpAsync(false));
        DebugEx.Log(TAG, "等待", await Tools.GetIpAsync());
    }
    
    #endregion

    #region GameObjectPoolTest

    private void GameObjectPoolTest()
    {
        var gObj = new GameObject("12312");
        var testGObjPool = ObjectPoolManager.Instance.Create<GameObject>("test", () => Instantiate(gObj), Destroy);
        
        for (var i = 0; i < 10; i++)
        {
            var go = ObjectPoolManager.Instance.Get<GameObject>("test");
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector2.zero;
            go.name = i.ToString();
            ObjectPoolManager.Instance.Put<GameObject>("test", go);
        }

        for (int i = 0; i < 100; i++)
        {
            var go = testGObjPool.Get();
            go.transform.SetParent(gameObject.transform);
            go.transform.localPosition = Vector2.zero;
            go.name = i.ToString();
            testGObjPool.Put(go);
        }

        ObjectPoolManager.Instance.Create<Dictionary<string, string>>("DictStrStr", () => new Dictionary<string, string>(), (a) => a.Clear());
        var dictStrStrPool = ObjectPoolManager.Instance.GetPool<Dictionary<string, string>>("DictStrStr");
        for (var i = 0; i < 10; i++)
        {
            var a = dictStrStrPool.Get();
            a.Add("test" + i, "test" + i);
            dictStrStrPool.Put(a);

            var b = dictStrStrPool.Get();
            dictStrStrPool.Put(b);
        }
        
        DebugEx.Log(dictStrStrPool.Count);
        dictStrStrPool.Clear();
        DebugEx.Log(dictStrStrPool.Count);
    }
    
    #endregion

    #region SoundManagerTest

    private void SoundManagerTest()
    {
        var go = transform.Find("SoundManagerTest").gameObject;
        go.SetActive(true);
        go.GetComponent<SoundManagerTest>().InitTest();
    }
    
    #endregion

    #region NativeBridgeTest

    private void NativeBridgeTest()
    {
#if UNITY_ANDROID
        DebugEx.Log(TAG, "NativeBridgeTest", NativeBridge.Instance.GetAndroidId());
        DebugEx.Log(TAG, "NativeBridgeTest", NativeBridge.Instance.GetRegionName());
        DebugEx.Log(TAG, "NativeBridgeTest", NativeBridge.Instance.GetLanguage());
#elif UNITY_IOS
        DebugEx.Log(TAG, "NativeBridgeTest", NativeBridge.Instance.GetUUID());
        DebugEx.Log(TAG, "NativeBridgeTest", NativeBridge.Instance.GetRegionName());
        DebugEx.Log(TAG, "NativeBridgeTest", NativeBridge.Instance.GetLanguage());
#endif
    }

    #endregion
     
}
