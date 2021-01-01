using System.Collections;
using System.Collections.Generic;
using IfeelgameFramework.Core.Logger;
using IfeelgameFramework.Core.Sound;
using UnityEngine;
using UnityEngine.UI;

public class SoundManagerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!_init)
        {
            Destroy(gameObject);
        }
    }

    private bool _init = false;
    public void InitTestButtons()
    {
        _init = true;
        
        SoundManager.InitData();
        
        var btnBase = transform.Find("btnBase").gameObject;
        var btnTextArr = new List<string>
        {
            "Play Music mp3",
            "Play Sound ogg",
            "Play Sound wav",
            "Play Sound mp3",
            "Pause/Resume Music",
            "Open/Close Music",
            "Open/Close Sound"
        };

        var audioNameArr = new List<string>
        {
            "bg",
            "beep",
            "market_sng_download_complete",
            "newblogtoast",
        };

        var offsetY = 150;
        var posY = btnTextArr.Count * (offsetY / 2f) - offsetY / 2f;
        for (var i = 0; i < btnTextArr.Count; i++)
        {
            var btnText = btnTextArr[i];
            var btn = Instantiate(btnBase, transform, false);
            btn.name = btnText;
            btn.transform.GetComponentInChildren<Text>().text = btnText;
            
            if (i <= 3)
            {
                var audioName = audioNameArr[i];
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    DebugEx.Log(audioName);

                    if (audioName == "bg")
                    {
                        SoundManager.PlayMusic(audioName);
                    }
                    else
                    {
                        SoundManager.PlaySound(audioName);
                    }
                    
                    UpdateBtnText();
                });
            }
            else
            {
                switch (i)
                {
                    case 4:
                        btn.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (SoundManager.GetMusicIsPlaying())
                            {
                                SoundManager.PauseBgm();
                            }
                            else
                            {
                                SoundManager.ResumeBgm();
                            }
                            
                            UpdateBtnText();
                        });
                        
                        _pauseResumeMusic = btn.transform.GetComponentInChildren<Text>();
                        break;
                    case 5:
                        btn.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (SoundManager.GetMusicEnabled())
                            {
                                SoundManager.SetMusicOff();
                            }
                            else
                            {
                                SoundManager.SetMusicOn();
                            }
                            
                            UpdateBtnText();
                        });

                        _closeOpenMusic = btn.transform.GetComponentInChildren<Text>();
                        break;
                    case 6:
                        btn.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (SoundManager.GetSoundEnabled())
                            {
                                SoundManager.SetSoundOff();
                            }
                            else
                            {
                                SoundManager.SetSoundOn();
                            }
                            
                            UpdateBtnText();
                        });

                        _closeOpenSound = btn.transform.GetComponentInChildren<Text>();
                        break;
                }
            }
            

            btn.transform.localPosition = new Vector2(0, posY);
            posY -= offsetY;
        }

        btnBase.gameObject.SetActive(false);

        var sliderMusicVolume = transform.Find("Music_Slider").GetComponent<Slider>();
        sliderMusicVolume.onValueChanged.AddListener(SoundManager.SetMusicVolume);
        
        var sliderSoundVolume = transform.Find("Sound_Slider").GetComponent<Slider>(); 
        sliderSoundVolume.onValueChanged.AddListener(SoundManager.SetSoundVolume);

        sliderMusicVolume.value = SoundManager.GetMusicVolume();
        sliderSoundVolume.value = SoundManager.GetSoundVolume();
        
        UpdateBtnText();
    }

    private Text _pauseResumeMusic;
    private Text _closeOpenMusic;
    private Text _closeOpenSound;
    private void UpdateBtnText()
    {
        _pauseResumeMusic.text = SoundManager.GetMusicIsPlaying() ? "Pause Music" : "Resume Music";
        _closeOpenMusic.text = SoundManager.GetMusicEnabled() ? "Close Music" : "Open Music";
        _closeOpenSound.text = SoundManager.GetSoundEnabled() ? "Close Sound" : "Open Sound";
    }
}
