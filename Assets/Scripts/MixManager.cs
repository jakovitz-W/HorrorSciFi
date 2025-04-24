using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixManager : MonoBehaviour
{
    private float masterLevel;

    [SerializeField] private AudioMixer mixer;

    public void SetMaster(float level){
        mixer.SetFloat("mastervol", Mathf.Log10(level) * 20);
        masterLevel = level;
    }

    public void SetSFXVolume(float level){
        mixer.SetFloat("sfxvol", Mathf.Log10(level) * 20);
    }

    public void SetMusicVolume(float level){
        mixer.SetFloat("musicvol", Mathf.Log10(level) * 20);
    }

    public void PauseMaster(){
        mixer.SetFloat("mastervol", 0.0001f);
    }

    public void ResumeMaster(){
        SetMaster(masterLevel);
    }
}
