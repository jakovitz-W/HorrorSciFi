using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secret : MonoBehaviour
{
    public GameObject roomEntry;

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.tag == "Player"){
            col.gameObject.transform.position = roomEntry.transform.position;
        }
    }
}
