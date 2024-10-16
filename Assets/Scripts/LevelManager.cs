using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //store stages in GameObject[] and set active/unactive to reduce lag
    public Level[] levels;
    public int LIndex; //level index
    public GameObject[] levelParents;
    public GameObject[] keyUI;
    private GameObject player;
    private int keyNum;
    public GameObject[] doors;
    public Animator transition;


    public IEnumerator OnRoomChange(int room){
        
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);

        LIndex = room;
        levelParents[room].SetActive(true);
        player.transform.position = levels[LIndex].checkpoints[0].transform.position;

        for(int i = 0; i < levels.Length; i++){
            if(i != room){
                levelParents[i].SetActive(false);
            }
        }
        yield return new WaitForSeconds(1);
        transition.SetTrigger("End");
    }

    public IEnumerator Backtrack(){

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        LIndex--;

        levelParents[LIndex].SetActive(true);
        player.transform.position = doors[LIndex].transform.position;

        for(int i = 0; i < levels.Length; i++){
            if(i != LIndex){
                levelParents[i].SetActive(false);
            }
        }
        yield return new WaitForSeconds(1);
        transition.SetTrigger("End");
        
    }

    public void ProcessCheckpoint(){
        levels[LIndex].CheckpointReached();
    }

    public GameObject GetCurrentCheckpoint(){
        return levels[LIndex].currentCheckpoint;
    }
    void Start(){
        player = GameObject.FindWithTag("Player");
        LIndex = 0; //for a level select use a set value based on the button pushed
        levels[LIndex].currentCheckpoint = levels[LIndex].checkpoints[0];
        levels[LIndex].chIndex = 0;


        for(int i = LIndex + 1; i < levels.Length; i++){ //deactivate all but current level
            levelParents[i].SetActive(false);
        }

        player.transform.position = levels[LIndex].checkpoints[0].transform.position;
    }

    public void ItemCollected(GameObject item){

        int index = Array.IndexOf(levels[LIndex].itemsWorld, item);
        keyUI[index].SetActive(true);
        keyNum++;
    }
}

[System.Serializable]
public class Level{
    public LevelManager manager;
    public GameObject[] checkpoints;
    public GameObject currentCheckpoint;
    public int chIndex  = 0; //checkpoint index
    public GameObject[] itemsWorld;
    
    public void CheckpointReached(){
        chIndex++;

        if(chIndex < (checkpoints.Length - 1)){
            currentCheckpoint = checkpoints[chIndex];
        }
    }
} 

