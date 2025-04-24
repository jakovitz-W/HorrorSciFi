using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private PlayerControls playerControls;
    private bool isActive = false;
    public GameObject pauseMenu;
    [SerializeField] private MixManager mixManager;

    private void Awake(){

        playerControls = new PlayerControls();        
        mixManager = GameObject.Find("MixManager").GetComponent<MixManager>();
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

            if(!isActive){
                AudioManager.Instance.PauseAll();
                pauseMenu.SetActive(true);
                isActive = true;                
                Cursor.visible = true;
                Time.timeScale = 0f;
                mixManager.PauseMaster();
            } else{
                Resume(false);
            }
        }
    }

    public void Resume(bool fromButton){ //in seperate function so button can call it
        if(fromButton){
            AudioManager.Instance.PlayUISound("button");
        }

        AudioManager.Instance.ResumeMusic();
        mixManager.ResumeMaster();
        pauseMenu.SetActive(false);
        isActive = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    public void Unstuck(){
        AudioManager.Instance.PlayUISound("button");
        PlayerMovement player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        player.OnDeath();
        AudioManager.Instance.ResumeMusic();
        mixManager.ResumeMaster();
        pauseMenu.SetActive(false);
    }

    public void Save(){
        AudioManager.Instance.PlayUISound("button");
        //grab important variables, load into JSONs
        Debug.Log("saving");
    }

    public void SaveAndQuit(){
        Save();
        CloseApp();
    }

    public void SaveAndMain(){
        Save();
        Time.timeScale = 1f;
        AudioManager.Instance.StopAll();
        SceneManager.LoadScene(1);
    }

    public void CloseApp(){
        AudioManager.Instance.PlayUISound("button");
        Application.Quit();
        Debug.Log("quit");
    }


}
