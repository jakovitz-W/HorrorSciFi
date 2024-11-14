using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col){
            Debug.Log("hitplate");
        if(col.gameObject.tag == "PressurePlate"){
            col.gameObject.GetComponent<PlateAbs>().plateFunction();
        }
    }
}
