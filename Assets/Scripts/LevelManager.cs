using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    //store stages in GameObject[] and set active/unactive to reduce lag
    public Level[] levels;
    private int LIndex; //level index
    public GameObject[] levelParents;
    public GameObject[] coinsUI;
    private GameObject player;
    public TextMeshProUGUI timerText;
    private bool timerActive;
    private float currentTime;

    public GameObject finishMenu;
    private int coinNum;
    public TextMeshProUGUI yourTimeText;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI coinsCollectedText;

    public void OnLevelChange(){

        if(levels[LIndex].bestTime == 0){
            levels[LIndex].bestTime = currentTime;
            //Debug.Log(levels[LIndex].bestTime);
        }   else if(levels[LIndex].bestTime > currentTime){
            levels[LIndex].bestTime = currentTime;
            //Debug.Log(levels[LIndex].bestTime);           
        }
        TimeSpan yourTime = TimeSpan.FromSeconds(currentTime);
        yourTimeText.text = "Your Time: " + yourTime.Minutes.ToString() + ":" + yourTime.Seconds.ToString();
            
        TimeSpan best = TimeSpan.FromSeconds(levels[LIndex].bestTime);
        bestTimeText.text = "Best Time: " + best.Minutes.ToString() + ":" + best.Seconds.ToString();

        coinsCollectedText.text = "Coins Collected: " + coinNum;
        
        LIndex++;
        if(LIndex != levels.Length){
            StartCoroutine(LoadNextLevel());

            levels[LIndex].currentCheckpoint = levels[LIndex].checkpoints[0];
            levels[LIndex].chIndex = 0;

            foreach(GameObject coin in coinsUI){
                coin.SetActive(false);
            }

            levelParents[LIndex].SetActive(true);
            levelParents[LIndex - 1].SetActive(false);

            player.transform.position = levels[LIndex].checkpoints[0].transform.position;
            timerText.text = "Time: " + 0;
            currentTime = 0;
            coinNum = 0;
        } else{
            finishMenu.SetActive(true);
            StartCoroutine(player.GetComponent<PlayerMovement>().OnFinish());
            timerActive = false;
        }
    }

    IEnumerator LoadNextLevel(){
        finishMenu.SetActive(true);
        yield return new WaitForSeconds(3);
        finishMenu.SetActive(false);
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

        foreach(GameObject coin in coinsUI){
            coin.SetActive(false);
        }

        for(int i = LIndex + 1; i < levels.Length; i++){ //deactivate all but current level
            levelParents[i].SetActive(false);
        }
        finishMenu.SetActive(false);
        player.transform.position = levels[LIndex].checkpoints[0].transform.position;

        levels[LIndex].bestTime = PlayerPrefs.GetFloat("bestTime", levels[LIndex].bestTime);
        Debug.Log(levels[LIndex].bestTime);
        timerActive = true;
    }

    void Update(){
        if(timerActive){
            currentTime = currentTime + Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timerText.text = "Time: " + time.Minutes.ToString() + ":" + time.Seconds.ToString();
    }

    public void CoinCollected(GameObject coin){

        int index = Array.IndexOf(levels[LIndex].coinsWorld, coin);
        coinsUI[index].SetActive(true);
        coinNum++;
    }
}

[System.Serializable]
public class Level{
    public LevelManager manager;
    public GameObject[] checkpoints;
    public GameObject currentCheckpoint;
    public int chIndex  = 0; //checkpoint index
    public GameObject[] coinsWorld;
    public float bestTime; //fix reset bug

    public void CheckpointReached(){
        chIndex++;

        if(chIndex < checkpoints.Length - 1){
            Debug.Log(chIndex);
            currentCheckpoint = checkpoints[chIndex];
        } else{
            manager.OnLevelChange();
        }
    }
} 

