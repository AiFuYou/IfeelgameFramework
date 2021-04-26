using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace IfeelgameFramework.Core.Sound
{
    public static class SoundManager
    {
        private static GameObject _soundManagerGameObject;

        private static SoundSettings _soundSettings;

        private static Dictionary<string, AudioSource> _musicAudioSourceDict;
        private static bool _multipleBgmEnabled;
        private static string _lastMusicBgmPath;
        private static Dictionary<string, AudioClip> _audioClips;
        private static AudioSource _soundAudioSource;
        
        /// <summary>
        /// 初始化音频模块
        /// </summary>
        /// <param name="multipleBgmEnabled">是否允许同时播放多个背景音乐，默认不允许</param>
        public static void InitData(bool multipleBgmEnabled = false)
        {
            //设置
            _soundSettings = new SoundSettings();
            
            //是否允许同时播放多个背景音乐
            _multipleBgmEnabled = multipleBgmEnabled;
            
            //初始化背景音乐表
            _musicAudioSourceDict = new Dictionary<string, AudioSource>();
            
            //音频片段表
            _audioClips = new Dictionary<string, AudioClip>();

            //添加SoundManagerComponent
            _soundManagerGameObject = new GameObject("SoundManager")
            {
                hideFlags = HideFlags.DontSave
            };
            _soundManagerGameObject.AddComponent<SoundManagerComponent>();

            //添加音效播放器
            _soundAudioSource = _soundManagerGameObject.AddComponent<AudioSource>();
            _soundAudioSource.playOnAwake = false;
            _soundAudioSource.loop = false;
            _soundAudioSource.volume = _soundSettings.SoundVolume;
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="bgmPath">背景音乐Path</param>
        /// <param name="loop">是否循环，默认循环</param>
        public static async void PlayMusic(string bgmPath, bool loop = true)
        {
            if (!string.IsNullOrEmpty(_lastMusicBgmPath) && _lastMusicBgmPath == bgmPath) return;

            if (!_multipleBgmEnabled && !string.IsNullOrEmpty(_lastMusicBgmPath))
            {
                if (_musicAudioSourceDict.ContainsKey(_lastMusicBgmPath))
                {
                    _musicAudioSourceDict[bgmPath] = _musicAudioSourceDict[_lastMusicBgmPath];
                    _musicAudioSourceDict.Remove(_lastMusicBgmPath);
                }
            }
            
            if (!_musicAudioSourceDict.ContainsKey(bgmPath))
            {
                _musicAudioSourceDict[bgmPath] = _soundManagerGameObject.AddComponent<AudioSource>();
            }

            _musicAudioSourceDict[bgmPath].clip = await Addressables.LoadAssetAsync<AudioClip>(bgmPath).Task;
            _musicAudioSourceDict[bgmPath].playOnAwake = false;
            _musicAudioSourceDict[bgmPath].loop = loop;
            _musicAudioSourceDict[bgmPath].volume = _soundSettings.MusicVolume;
            _musicAudioSourceDict[bgmPath].Play();

            if (!GetMusicEnabled())
            {
                PauseBgm(bgmPath);
            }

            _lastMusicBgmPath = bgmPath;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundPath">音效Path</param>
        /// <param name="volume">音效音量</param>
        public static async void PlaySound(string soundPath, float volume = 1.0f)
        {
            if (!GetSoundEnabled()) return;
            
            AudioClip audioClip;
            if (_audioClips.ContainsKey(soundPath))
            {
                audioClip = _audioClips[soundPath];
            }
            else
            {
                audioClip = await Addressables.LoadAssetAsync<AudioClip>(soundPath).Task;
                _audioClips.Add(soundPath, audioClip);
            }

            _soundAudioSource.PlayOneShot(audioClip, volume);
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        /// <param name="bgmPath">暂停指定的背景音乐，若传空则停止所有背景音乐</param>
        public static void PauseBgm(string bgmPath = null)
        {
            if (bgmPath != null)
            {
                if (_musicAudioSourceDict.ContainsKey(bgmPath))
                {
                    _musicAudioSourceDict[bgmPath].Pause();    
                }
            }
            else
            {
                foreach (var item in _musicAudioSourceDict)
                {
                    item.Value.Pause();
                }
            }
        }

        /// <summary>
        /// 继续播放背景音乐
        /// </summary>
        /// <param name="bgmPath">继续播放指定背景音乐，若传空则继续播放所有背景音乐</param>
        public static void ResumeBgm(string bgmPath = null)
        {
            if (bgmPath != null)
            {
                if (_musicAudioSourceDict.ContainsKey(bgmPath) && !_musicAudioSourceDict[bgmPath].isPlaying)
                {
                    _musicAudioSourceDict[bgmPath].UnPause();    
                }
            }
            else
            {
                foreach (var item in _musicAudioSourceDict)
                {
                    if (!item.Value.isPlaying)
                    {
                        item.Value.UnPause();
                    }
                }   
            }
        }

        /// <summary>
        /// 背景音乐是否在播放
        /// </summary>
        /// <param name="bgmPath">制定背景音乐是否在播放，若传空则判定是否有背景音乐在播放</param>
        /// <returns></returns>
        public static bool GetMusicIsPlaying(string bgmPath = null)
        {
            if (bgmPath != null)
            {
                if (_musicAudioSourceDict.ContainsKey(bgmPath) && _musicAudioSourceDict[bgmPath].isPlaying)
                {
                    return true;
                }
            }

            foreach (var item in _musicAudioSourceDict)
            {
                if (item.Value.isPlaying)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// 打开背景音乐开关
        /// </summary>
        public static void SetMusicOn()
        {
            _soundSettings.MusicEnabled = true;

            foreach (var item in _musicAudioSourceDict)
            {
                item.Value.mute = false;
                item.Value.Play();
            }
        }

        /// <summary>
        /// 关闭背景音乐开关
        /// </summary>
        public static void SetMusicOff()
        {
            _soundSettings.MusicEnabled = false;

            foreach (var item in _musicAudioSourceDict)
            {
                item.Value.mute = true;
                item.Value.Stop();
            }
        }

        /// <summary>
        /// 获取背景音乐开关
        /// </summary>
        /// <returns></returns>
        public static bool GetMusicEnabled()
        {
            return _soundSettings.MusicEnabled;
        }

        /// <summary>
        /// 打开音效开关
        /// </summary>
        public static void SetSoundOn()
        {
            _soundSettings.SoundEnabled = true;
        }

        /// <summary>
        /// 关闭音效开关
        /// </summary>
        public static void SetSoundOff()
        {
            _soundSettings.SoundEnabled = false;
        }

        /// <summary>
        /// 获取音效开关
        /// </summary>
        /// <returns></returns>
        public static bool GetSoundEnabled()
        {
            return _soundSettings.SoundEnabled;
        }
    
        /// <summary>
        /// 设置背景音乐音量
        /// </summary>
        /// <param name="volume">音量值</param>
        public static void SetMusicVolume(float volume)
        {
            _soundSettings.MusicVolume = volume;
            foreach (var item in _musicAudioSourceDict)
            {
                item.Value.volume = _soundSettings.MusicVolume;
            }
        }

        /// <summary>
        /// 获取背景音乐音量值
        /// </summary>
        /// <returns></returns>
        public static float GetMusicVolume()
        {
            return _soundSettings.MusicVolume;
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        /// <param name="volume"></param>
        public static void SetSoundVolume(float volume)
        {
            _soundSettings.SoundVolume = volume;
            _soundAudioSource.volume = _soundSettings.SoundVolume;
        }

        /// <summary>
        /// 获取音效音量
        /// </summary>
        /// <returns></returns>
        public static float GetSoundVolume()
        {
            return _soundSettings.SoundVolume;
        }
    }
}