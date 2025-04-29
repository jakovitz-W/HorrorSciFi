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
    [SerializeField] private PlayerInteractions interactions;

    private void Awake(){

        playerControls = new PlayerControls();
        interactions = GameObject.FindWithTag("Player").GetComponent<PlayerInteractions>();
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
                interactions.StopToolAudio();
                interactions.enabled = false;
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

        interactions.enabled = true;
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
        Resume(false);
    }

    public void Main(){
        Time.timeScale = 1f;
        mixManager.ResumeMaster();
        AudioManager.Instance.StopAll();
        SceneManager.LoadScene(0);
    }

    public void CloseApp(){
        AudioManager.Instance.PlayUISound("button");
        Application.Quit();
        Debug.Log("quit");
    }


}
