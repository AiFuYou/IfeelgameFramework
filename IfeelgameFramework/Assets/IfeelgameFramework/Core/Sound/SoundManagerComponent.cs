using System;
using UnityEngine;

namespace IfeelgameFramework.Core.Sound
{
    public class SoundManagerComponent : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
