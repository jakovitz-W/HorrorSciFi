using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UpgradeSystem : MonoBehaviour
{   
    private PlayerControls playerControls;    
    [SerializeField] private PauseMenu pauseMenu;
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
    [SerializeField] private TMP_Text tokens;

    private bool regenUnlocked = false;
    [SerializeField] private GameObject regen, speed, range, stun, drone; //buttons


    public void DebugAdd(){
        humansSaved++;
        UpdateWallet();
    }
    
    void Awake(){
        playerControls = new PlayerControls();
        player = GameObject.FindWithTag("Player");
        movement = player.GetComponent<PlayerMovement>();
        interactions = player.GetComponent<PlayerInteractions>();
    }

    void OnEnable(){
        playerControls.Land.Interact.performed += CloseMenu;
        playerControls.Enable();
    }

    void OnDisable(){
        playerControls.Land.Interact.performed -= CloseMenu;
        playerControls.Disable();
    }
    
    public void OpenMenu(){
        
        wallet = humansSaved - spent;
        tokens.text = "Tokens: " + wallet;
        if(movement.hasDroneKey){
            drone.SetActive(true);
        } else{
            drone.SetActive(false);
        }

        if(!upgradeMenu.activeSelf){
            movement.enabled = false;
            interactions.enabled = false;
            pauseMenu.enabled = false;
            Cursor.visible = true;
            upgradeMenu.SetActive(true);   
            Recalculate();
            Time.timeScale = 0f;     
        }
    }

    public void CloseMenu(InputAction.CallbackContext ctx){
        if(ctx.performed){

            if(!upgradeMenu.activeSelf){
                return;
            }
            pauseMenu.enabled = true;
            movement.enabled = true;
            interactions.enabled = true;
            Cursor.visible = false;

            upgradeMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void UpdateWallet(){
        wallet = humansSaved - spent;        
    }

    void Recalculate(){

        UpdateWallet();
        tokens.text = "Tokens: " + wallet;

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
        AudioManager.Instance.PlayUISound("terminal");
        StartCoroutine(movement.RegenSpeed());
        regenUnlocked = true;
        spent += regenCost;
        Recalculate();
    }

    public void IncreaseBaseSpeed(){
        AudioManager.Instance.PlayUISound("terminal");
        movement.baseSpeed += speedIncrease;
        movement.runSpeed = movement.baseSpeed;
        spent += speedCost;
        Recalculate();
    }

    public void IncreaseRange(){
        AudioManager.Instance.PlayUISound("terminal");
        interactions.attackRadius += attackUpgrade;
        spent += rangeCost;
        Recalculate();
    }

    public void IncreaseStunDuration(){
        AudioManager.Instance.PlayUISound("terminal");
        interactions.stunTime += stunUpgrade;
        spent += durationCost;
        Recalculate();
    }

    public void UnlockMiniDrone(){
        AudioManager.Instance.PlayUISound("terminal");
        movement.droneUnlocked = true;
        diSystem.SetText("MiniDrone", true, false, -1);
        drone.SetActive(false);
        movement.hasDroneKey = false;
    }
}
