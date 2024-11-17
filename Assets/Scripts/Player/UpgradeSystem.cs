using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerInteractions interactions;
    [SerializeField] private DialogueSystem diSystem;

    [SerializeField] private float regenPercent = .05f;
    [SerializeField] private float regenStep = 2f;
    [SerializeField] private float speedIncrease = 3f;
    [SerializeField] private float attackUpgrade = .3f;
    [SerializeField] private float stunUpgrade = .5f;


    public IEnumerator RegenSpeed(){

        while(movement.runSpeed <= movement.baseSpeed){
            movement.runSpeed += (movement.runSpeed * regenPercent);
            yield return new WaitForSeconds(regenStep);
            StartCoroutine(RegenSpeed());
        }
    }

    public void IncreaseBaseSpeed(){
        movement.baseSpeed += speedIncrease;
        movement.runSpeed = movement.baseSpeed;
    }

    public void IncreaseRange(){
        interactions.attackRadius += attackUpgrade;
        //increase the lifetime of the flame/taser particles
    }

    public void IncreaseStunDuration(){
        interactions.stunTime += stunUpgrade;
    }

    public void UnlockMiniDrone(){
        movement.droneUnlocked = true;
        diSystem.SetText("MiniDrone", true, false);
    }
}
