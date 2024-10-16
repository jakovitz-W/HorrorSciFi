using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //store stages in GameObject[] and set active/unactive to reduce lag
    public Level[] levels;
    public GameObject[] keyUI;
    private GameObject player;
    [HideInInspector] public int LIndex; //level index
    [SerializeField] private GameObject[] levelParents;  
    [SerializeField] private GameObject[] doors;
    [SerializeField] private Animator transition;
    

    public IEnumerator OnRoomChange(int room){
        
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);

        levelParents[room].SetActive(true);
        player.transform.position = levels[room].checkpoint.transform.position;

        for(int i = 0; i < levels.Length; i++){
            if(i != room){
                levelParents[i].SetActive(false);
            }
        }

        LIndex++;

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

    void Start(){

        player = GameObject.FindWithTag("Player");
        LIndex = 0;

        for(int i = LIndex + 1; i < levels.Length; i++){ //deactivate all but current level
            levelParents[i].SetActive(false);
            levels[i].hasKey = false;
        }

        player.transform.position = levels[0].checkpoint.transform.position;
    }
}


[System.Serializable]
public class Level{
    public GameObject checkpoint;
    public bool hasKey; 
} 

