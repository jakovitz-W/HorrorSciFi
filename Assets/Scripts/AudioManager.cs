using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] music, sfxSounds, UISounds;

    public AudioSource musicSource, sfxSource, UISource, globalSource;

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

    public void PauseAll(){
        musicSource.Pause();
        sfxSource.Pause();
    }

    public void ResumeMusic(){
        musicSource.Play();
    }

    public void StopAll(){
        sfxSource.Stop();
        musicSource.Stop();
        UISource.Stop();
    }

    //global
    public void PlaySFX(string name){
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if(s == null){
            Debug.Log("Sound not found");
        } else{
            globalSource.PlayOneShot(s.clip);
        }
    }

    //localized
    public void PlaySFXAtPoint(string name, Transform spawnPoint){

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if(s == null){
            Debug.Log("sound not found");
        } else{
            AudioSource audioSource = Instantiate(sfxSource, spawnPoint);
            audioSource.clip = s.clip;
            audioSource.PlayOneShot(s.clip);
            
            float clipLength = audioSource.clip.length;

            Destroy(audioSource.gameObject, clipLength);            
        }
    }

    public AudioSource PlayRepeatingGlobal(string name){
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if(s == null){
            Debug.Log("sound not found");
            return null;
        } else{
            AudioSource audioSource = Instantiate(globalSource, this.transform.position, Quaternion.identity);
            audioSource.clip = s.clip;
            audioSource.loop = true;
            audioSource.Play();
            return audioSource;
        }
    }

    public AudioSource PlayRepeatingAtPoint(string name, Transform spawnPoint){

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if(s == null){
            Debug.Log("sound not found");
            return null;
        } else{
            AudioSource audioSource = Instantiate(sfxSource, spawnPoint);
            audioSource.clip = s.clip;

            audioSource.loop = true;
            audioSource.Play();
            return audioSource;
        }
    }

    public void PlayUISound(string name){
        Sound s = Array.Find(UISounds, x => x.name == name);

        if(s == null){
            Debug.Log("Sound not found");
        } else{
            UISource.PlayOneShot(s.clip);
        }
    }
}
