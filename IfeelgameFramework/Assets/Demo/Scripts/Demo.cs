using System.Globalization;
using IfeelgameFramework.Core.Logger;
using IfeelgameFramework.Core.ObjectPool;
using IfeelgameFramework.Core.Sound;
using IfeelgameFramework.Core.Utils;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.InitData();
        SoundManager.PlayMusic("bg");
        
        var gObj = new GameObject("12312");
        GameObjectPool.Instance.Add("test", gObj);
        GameObjectPool.Instance.Add("test", gObj);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
