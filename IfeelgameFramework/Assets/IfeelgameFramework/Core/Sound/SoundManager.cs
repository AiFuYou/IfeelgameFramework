using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IfeelgameFramework.Core.Sound
{
    public static class SoundManager
    {
        private static GameObject _soundManagerGameObject;
    
        private static string _musicEnabled = "on";
        private static float _musicVolume = 1.0f;
        private static string _soundEnabled = "on";
        private static float _soundVolume = 1.0f;

        private static AudioSource _musicAudioSource;
        private static List<AudioSource> _unusedAudioSources;
        private static List<AudioSource> _usedAudioSources;
        private static Dictionary<string, AudioClip> _audioClips;
        private const int MaxCount = 7;
        
        public static void InitData()
        {
            //空闲播放器列表
            _unusedAudioSources = new List<AudioSource>();
            //正在使用播放器列表
            _usedAudioSources = new List<AudioSource>();
            //音频片段表
            _audioClips = new Dictionary<string, AudioClip>();
        
            //添加音频播放组件
            _soundManagerGameObject = new GameObject("SoundManager")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            _soundManagerGameObject.AddComponent<SoundManagerComponent>();
            
        }

        //播放器回收
        private static void UsedToUnused(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                if (_usedAudioSources.Contains(audioSource))
                {
                    _usedAudioSources.Remove(audioSource);
                }

                if (!_unusedAudioSources.Contains(audioSource))
                {
                    if (_unusedAudioSources.Count >= MaxCount)
                    {
                        Object.Destroy(audioSource);
                    }
                    else
                    {
                        _unusedAudioSources.Add(audioSource);
                    }
                }
            }
        }

        //取出播放器
        private static AudioSource UnusedToUsed()
        {
            AudioSource audioSource;
            if (_unusedAudioSources.Count > 0)
            {
                audioSource = _unusedAudioSources[0];
                _unusedAudioSources.RemoveAt(0);
            }
            else
            {
                audioSource = _soundManagerGameObject.AddComponent<AudioSource>();
            }
            _usedAudioSources.Add(audioSource);
            return audioSource;
        }

        //等待播放器播放完成
        private static IEnumerator WaitPlayEnd(AudioSource audioSource, Action action = null)
        {
            yield return new WaitUntil(() => !audioSource.isPlaying);
            UsedToUnused(audioSource);
            action?.Invoke();
        }

        //播放背景音乐
        public static void PlayMusic(string bgmPath, float volume = 0.0f)
        {
            if (_musicAudioSource == null)
            {
                _musicAudioSource = UnusedToUsed();
            }
            
            _musicAudioSource.clip = Resources.Load<AudioClip>(bgmPath);
            _musicAudioSource.playOnAwake = false;
            _musicAudioSource.loop = true;
            _musicAudioSource.volume = volume > 0.0f ? volume : _musicVolume;
            _musicAudioSource.Play();

            if (!GetMusicEnabled())
            {
                _musicAudioSource.mute = true;
                _musicAudioSource.Pause();
            }
        }

        //播放音效
        public static void PlaySound(string soundPath, float volume = 0.0f)
        {
            if (!GetSoundEnabled()) return;
            var audioSource = UnusedToUsed();

            AudioClip audioClip;
            if (_audioClips.ContainsKey(soundPath))
            {
                audioClip = _audioClips[soundPath];
            }
            else
            {
                audioClip = Resources.Load<AudioClip>(soundPath);
                _audioClips.Add(soundPath, audioClip);
            }

            audioSource.clip = audioClip;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = volume > 0.0f? volume: _soundVolume;
            audioSource.Play();
            _soundManagerGameObject.GetComponent<SoundManagerComponent>().StartCoroutine(WaitPlayEnd(audioSource));
        }

        //暂停背景音乐
        public static void PauseBgm()
        {
            if (_musicAudioSource != null && _musicAudioSource.isPlaying)
            {
                _musicAudioSource.Pause();
            }
        }

        //继续背景音乐
        public static void ResumeBgm()
        {
            if (_musicAudioSource != null && !_musicAudioSource.isPlaying)
            {
                _musicAudioSource.Play();
            }
        }

        public static void SetMusicOn()
        {
            _musicEnabled = "on";

            if (_musicAudioSource != null)
            {
                _musicAudioSource.mute = false;
                _musicAudioSource.Play();
            }
        }

        public static void SetMusicOff()
        {
            _musicEnabled = "off";
        
            if (_musicAudioSource != null)
            {
                _musicAudioSource.mute = true;
                _musicAudioSource.Pause();
            }
        }

        private static bool GetMusicEnabled()
        {
            return _musicEnabled == "on";
        }

        public static void SetSoundOn()
        {
            _soundEnabled = "on";
        }

        public static void SetSoundOff()
        {
            _soundEnabled = "off";
        }

        private static bool GetSoundEnabled()
        {
            return _soundEnabled == "on";
        }
    
        public static void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
            if (_musicAudioSource != null && _musicAudioSource.isPlaying)
            {
                _musicAudioSource.volume = _musicVolume;
            }
        }

        public static void SetSoundVolume(float volume)
        {
            _soundVolume = volume;
        }
    }
}