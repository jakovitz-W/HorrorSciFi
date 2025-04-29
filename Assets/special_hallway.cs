using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a lazy non-scaleable way to do this, do not do this
//set wait time in inspector or add a "special" bool to the level in levelmanager
public class special_hallway : MonoBehaviour
{
    public GameObject checkpoint;
    void OnEnable(){
        StartCoroutine(EnableCheckpoint());
    }

    private IEnumerator EnableCheckpoint(){
        checkpoint.SetActive(false);
        yield return new WaitForSeconds(18f);
        checkpoint.SetActive(true);
        this.enabled = false;
    }
}
