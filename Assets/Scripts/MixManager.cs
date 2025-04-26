using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixManager : MonoBehaviour
{
    private float masterLevel, sfxLevel, musicLevel;

    [SerializeField] private AudioMixer mixer;

    public void SetMaster(float level){
        mixer.SetFloat("mastervol", Mathf.Log10(level) * 20);
        masterLevel = level;
    }

    public void SetSFXVolume(float level){
        mixer.SetFloat("sfxvol", Mathf.Log10(level) * 20);
        sfxLevel = level;
    }

    public void SetMusicVolume(float level){
        mixer.SetFloat("musicvol", Mathf.Log10(level) * 20);
        musicLevel = level;
    }

    public void SetUIVolume(float level){
        mixer.SetFloat("UIvol", Mathf.Log10(level) * 20);
    }

    public void PauseMaster(){
        mixer.SetFloat("sfxvol", -80f);
        mixer.SetFloat("musicvol", -80f);
    }

    public void ResumeMaster(){
        SetSFXVolume(sfxLevel);
        SetMusicVolume(musicLevel);
    }
}
