using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //remember to set an initial spawnpoint in the world
    private LevelManager manager;
    public Sprite claimedSprite;
    public bool finishLine; //set flag type in inspector
    private Collider2D collider;
    private bool hit = false;
    void Start(){
        manager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        collider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other){

        if(other.gameObject.CompareTag("Player") && !hit){
            hit = true; //keeps collision from triggering more than once
            collider.enabled = false;
            if(!finishLine){
                GetComponent<SpriteRenderer>().sprite = claimedSprite;
            }
            
            manager.ProcessCheckpoint();
        }
    }
}
