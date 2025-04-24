using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col){
            
        if(col.gameObject.tag == "PressurePlate"){
            col.gameObject.GetComponent<ButtonScript>().ActivateAll();
        }
    }

    void OnCollisionEnter2D(Collision2D col){

        if(col.gameObject.tag == "magnet"){
            col.gameObject.GetComponent<PointEffector2D>().enabled = false;
        }
    }
}
