using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeScript : MonoBehaviour
{
    public Animator jumpscare;
    private AudioSource audioSource;
    private bool open = false;

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "Player"){
            if(!open){
                audioSource.Pause();
                jumpscare.SetTrigger("PlayerEnter");
                AudioManager.Instance.PlaySFXAtPoint("fridge_burst", this.transform);
                open = true;                
            }

        }
    }

    void OnEnable(){
        audioSource = AudioManager.Instance.PlayRepeatingAtPoint("fridge_rattle", this.transform);
    }
}
