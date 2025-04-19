using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField] private GameObject affectedDoor;

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "crate"){
            affectedDoor.GetComponent<Animator>().SetTrigger("open");
        }
    }
}
