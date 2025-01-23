using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    private GameObject player;
    private PlayerMovement movement;
    private PlayerInteractions interactions;
    [SerializeField] private DialogueSystem diSystem;

    [SerializeField] private float speedIncrease = 3f;
    [SerializeField] private float attackUpgrade = .3f;
    [SerializeField] private float stunUpgrade = .5f;


    [SerializeField] private float speedCap, durationCap, rangeCap;

    [SerializeField] private GameObject upgradeMenu;
    private bool allowDroneUnlock = false;
    private int spent = 0; //how much the player has spent
    private int wallet = 0; //how much currency the player has

    private bool regenUnlocked, speedUnlock, rangeUnlock, stunUnlock = false;
    [SerializeField] private GameObject regen, speed, range, stun, drone; //buttons

    void Awake(){
        player = GameObject.FindWithTag("Player");
        movement = player.GetComponent<PlayerMovement>();
        interactions = player.GetComponent<PlayerInteractions>();
    }
    
    public void OpenMenu(){
        //wallet = interactions.humansSaved - spent;

        if(movement.hasDroneKey){
            drone.SetActive(true);
        } else{
            drone.SetActive(false);
        }

        if(!upgradeMenu.activeSelf){
            upgradeMenu.SetActive(true);   
            Time.timeScale = 0f;     
        } else{
            upgradeMenu.SetActive(false);
            Time.timeScale = 1f;
        }

    }

    public void StartRegen(){
        StartCoroutine(movement.RegenSpeed());
        regen.SetActive(false);
    }

    public void IncreaseBaseSpeed(){
        movement.baseSpeed += speedIncrease;
        movement.runSpeed = movement.baseSpeed;
        
        if(movement.baseSpeed >= speedCap){
            speed.SetActive(false);
        }
    }

    public void IncreaseRange(){
        interactions.attackRadius += attackUpgrade;
        //increase the lifetime of the flame/taser particles

        if(interactions.attackRadius >= rangeCap){
            range.SetActive(false);
        }
    }

    public void IncreaseStunDuration(){
        interactions.stunTime += stunUpgrade;

        if(interactions.stunTime >= durationCap){
            stun.SetActive(false);
        }
    }

    public void UnlockMiniDrone(){
        movement.droneUnlocked = true;
        diSystem.SetText("MiniDrone", true, false);
        drone.SetActive(false);
        movement.hasDroneKey = false;
    }
}
