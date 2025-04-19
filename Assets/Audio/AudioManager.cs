using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] music, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake(){
        
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else{
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name){
        Sound s = Array.Find(music, x => x.name == name);

        if(s == null){
            Debug.Log("Sound not found");
        } else{
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name){
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if(s == null){
            Debug.Log("Sound not found");
        } else{
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
