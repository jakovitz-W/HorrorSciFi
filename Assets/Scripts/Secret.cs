using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secret : MonoBehaviour
{
    public GameObject room;

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.tag == "Player"){
            col.gameObject.transform.position = room.transform.position;
        }
    }
}
