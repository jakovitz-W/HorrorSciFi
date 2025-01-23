using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private PlayerControls playerControls;
    private bool isActive = false;
    public GameObject pauseMenu;

    private void Awake(){
        playerControls = new PlayerControls();
        isActive = false;
        pauseMenu.SetActive(false);
    }

    private void OnEnable(){
        playerControls.General.Pause.performed += Pause;
        playerControls.Enable();
    }

    private void OnDisable(){
        playerControls.General.Pause.performed -= Pause; 
        playerControls.Disable();
    }

    void Pause(InputAction.CallbackContext ctx){
        
        if(ctx.performed){
            isActive = !isActive;
            pauseMenu.SetActive(isActive);

            if(isActive){
                Cursor.visible = true;
                Time.timeScale = 0f;
            } else{
                Resume();
            }
        }
    }

    public void Resume(){ //in seperate function so button can call it
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    public void Quit(){
        Application.Quit();
    }

    public void ResetRoom(){
        //play loading animation
        //grab room
        //grab initial state of the room?
        //set all critical objects back to their initial position/state
            //if critical object position/state has changed then reset them
            //else skip
        //Set player position
        //stop loading animation
    }

}
