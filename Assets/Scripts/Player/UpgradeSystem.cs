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
    [SerializeField] private int regenCost = 1, speedCost = 2, durationCost = 1, rangeCost = 3;

    [SerializeField] private GameObject upgradeMenu;
    public int humansSaved = 0;
    private int spent = 0;
    public int wallet = 0; //how much currency the player has

    private bool regenUnlocked = false;
    [SerializeField] private GameObject regen, speed, range, stun, drone; //buttons

    void Awake(){
        player = GameObject.FindWithTag("Player");
        movement = player.GetComponent<PlayerMovement>();
        interactions = player.GetComponent<PlayerInteractions>();
    }
    
    public void OpenMenu(){
        wallet = humansSaved - spent;

        if(movement.hasDroneKey){
            drone.SetActive(true);
        } else{
            drone.SetActive(false);
        }

        if(!upgradeMenu.activeSelf){
            upgradeMenu.SetActive(true);   
            Recalculate();
            Time.timeScale = 0f;     
        } else{
            upgradeMenu.SetActive(false);
            Time.timeScale = 1f;
        }

    }

    void Recalculate(){
        wallet = humansSaved - spent;

        if(wallet < regenCost || regenUnlocked){
            regen.SetActive(false);
        } else{
            regen.SetActive(true);
        }

        if(wallet < speedCost || movement.baseSpeed >= speedCap){
            speed.SetActive(false);
        } else{
            speed.SetActive(true);
        }

        if(wallet < durationCost || interactions.stunTime >= durationCap){
            stun.SetActive(false);
        }else{
            stun.SetActive(true);
        }

        if(wallet < rangeCost || interactions.attackRadius >= rangeCap){
            range.SetActive(false);
        } else{
            range.SetActive(true);
        }
    }

    public void StartRegen(){
        StartCoroutine(movement.RegenSpeed());
        regenUnlocked = true;
        spent += regenCost;
        Recalculate();
    }

    public void IncreaseBaseSpeed(){
        movement.baseSpeed += speedIncrease;
        movement.runSpeed = movement.baseSpeed;
        spent += speedCost;
        Recalculate();
    }

    public void IncreaseRange(){
        interactions.attackRadius += attackUpgrade;
        //increase the lifetime of the flame/taser particles
        spent += rangeCost;
        Recalculate();
    }

    public void IncreaseStunDuration(){
        interactions.stunTime += stunUpgrade;
        spent += durationCost;
        Recalculate();
    }

    public void UnlockMiniDrone(){
        movement.droneUnlocked = true;
        diSystem.SetText("MiniDrone", true, false);
        drone.SetActive(false);
        movement.hasDroneKey = false;
    }
}
