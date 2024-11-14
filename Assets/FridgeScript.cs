using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeScript : MonoBehaviour
{
    public Animator jumpscare;

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "Player"){
            jumpscare.SetTrigger("PlayerEnter");
        }
    }
}
