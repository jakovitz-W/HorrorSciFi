using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugMenu : MonoBehaviour
{
    private PlayerControls playerControls;
    private bool isActive = false;
    public GameObject debugMenu;
    public GameObject player;
    public LevelManager levelManager;
    private bool collidersEnabled = true;
    private bool inTestRoom = false;

    private void Awake(){
        //playerControls = new PlayerControls();
        isActive = false;
        debugMenu.SetActive(false);
    }

    private void OnEnable(){
        //playerControls.General.OpenDebugMenu.performed += OpenDebugMenu;
        //playerControls.Enable();
    }

    private void OnDisable(){
        //playerControls.General.OpenDebugMenu.performed -= OpenDebugMenu; 
        //playerControls.Disable();
    }


    void OpenDebugMenu(InputAction.CallbackContext ctx){

        if(ctx.performed){
            isActive = !isActive;
            debugMenu.SetActive(isActive);
        }

        if(isActive){
            inTestRoom = false;
            Cursor.visible = true;
            Time.timeScale = 0f;
        } else{
            Resume();
        }
    }

    public void OpenDebug(){
        isActive = !isActive;
        debugMenu.SetActive(isActive);
        Cursor.visible = true;
        inTestRoom = true;
    }

    public void Resume(){
        debugMenu.SetActive(false);
        Time.timeScale = 1f;
        
        if(!inTestRoom){
            Cursor.visible = false;            
        }

    }

    public void UnlockDoors(){
        for(int i = 0; i < levelManager.levels.Length; i++){
            levelManager.levels[i].hasKey = true;
        }
    }

    public void UnlockWeapons(){
        player.GetComponent<PlayerInteractions>().hasTorch = true;
        player.GetComponent<PlayerInteractions>().hasTaser = true;
        player.GetComponent<PlayerInteractions>().torchActive = true;
    }

    public void UnlockDrone(){
        player.GetComponent<PlayerMovement>().droneUnlocked = true;
    }

    public void SkipToBoss(){
        UnlockWeapons();
        UnlockDrone();
        UnlockDoors();
        player.GetComponent<PlayerInteractions>().hasCure = true;

        StartCoroutine(levelManager.OnRoomChange(5));
    }

    public void SkipToLab(){
        UnlockWeapons();
        UnlockDoors();

        StartCoroutine(levelManager.OnRoomChange(4));
    }

    public void SkipToMedbay(){
        UnlockWeapons();
        UnlockDoors();

        StartCoroutine(levelManager.OnRoomChange(2));
    }

    public void ToggleCollision(){
        collidersEnabled = !collidersEnabled;
        
        foreach(Collider2D col in player.GetComponents<Collider2D>()){
            col.enabled = collidersEnabled;
        }
    }
}
