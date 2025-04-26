using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneManagement : MonoBehaviour
{   
    private PlayerControls playerControls;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject[] subMenus;
    private GameObject activeSubMenu;

    [SerializeField] private GameObject[] optionMenus;
    private GameObject activeOption;


    private void Awake(){
        playerControls = new PlayerControls();
    }
    private void OnEnable(){
        playerControls.General.Pause.performed += CloseSubMenu;
        playerControls.Enable();
    }

    private void OnDisable(){
        playerControls.General.Pause.performed -= CloseSubMenu; 
        playerControls.Disable();
    }
    public void StartGame(){
        AudioManager.Instance.PlayUISound("button");
        SceneManager.LoadScene(1);
    }

    public void QuitGame(){
        Debug.Log("Quit");
        Application.Quit();
    }

    //assign index from the button calling it in the inspector
    public void OpenOption(int index){

        AudioManager.Instance.PlayUISound("button");
        //note:acc = 0, audio= 1, ctrls = 2
        for(int i = 0; i < optionMenus.Length; i++){
            optionMenus[i].SetActive(false);
        }

        activeOption = optionMenus[index];
        activeOption.SetActive(true);
    }

    public void OpenSubMenu(int index){
        AudioManager.Instance.PlayUISound("button");
        //note:options = 0, confirmation = 1
        activeSubMenu = subMenus[index];
        activeSubMenu.SetActive(true);       
    }

    //bound to escape
    public void CloseSubMenu(InputAction.CallbackContext ctx){
        
        if(ctx.performed){

            if(activeSubMenu != null){
                activeSubMenu.SetActive(false);
                activeSubMenu = null;
            }

            for(int i = 0; i < subMenus.Length; i++){
                if(subMenus[i].activeSelf){
                    activeSubMenu = subMenus[i];
                    break;
                }
            }

            if(credits != null && credits.activeSelf){
                AudioManager.Instance.StopAll();
                credits.SetActive(false);
            }
        }
    }

    //same as close but bound to a button
    public void Back(){
        AudioManager.Instance.PlayUISound("button");
        if(activeSubMenu != null){
            activeSubMenu.SetActive(false);
            activeSubMenu = null;
        }

        for(int i = 0; i < subMenus.Length; i++){
            if(subMenus[i].activeSelf){
                activeSubMenu = subMenus[i];
                break;
            }
        }
    }

    public void Credits(){
        credits.SetActive(true);
    }
}
