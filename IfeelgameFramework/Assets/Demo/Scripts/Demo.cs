using System.Globalization;
using IfeelgameFramework.Core.Logger;
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
        
        DebugEx.Log("当前国家", NativeBridge.Instance.GetRegionName());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
