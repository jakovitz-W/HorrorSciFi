using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilWalls : MonoBehaviour
{
    [SerializeField] private Transform respawn;
    private Collider2D col;
    [SerializeField] private float timer = 1f; //default interval, could make random
    private SpriteRenderer rend;

    void OnEnable(){
        col = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();
        StartCoroutine(Timer());
    }

    void OnTriggerEnter2D(Collider2D col){

        if(col.gameObject.tag == "Player"){
            col.gameObject.transform.position = respawn.position;
        }
    }

    private IEnumerator Timer(){

        col.enabled = false;
        rend.color = new Color(0f, 1f, 0f, .5f);

        yield return new WaitForSeconds(timer);
        col.enabled = true;
        rend.color = new Color(1f, 0f, 0f, .5f);
        yield return new WaitForSeconds(timer);

        StartCoroutine(Timer());
    }
}
