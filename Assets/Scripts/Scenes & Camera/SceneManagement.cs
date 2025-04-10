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

    }

    public void QuitGame(){
        Debug.Log("Quit");
        Application.Quit();
    }

    //assign index from the button calling it in the inspector
    public void OpenOption(int index){

        //note:acc = 0, audio= 1, ctrls = 2
        for(int i = 0; i < optionMenus.Length; i++){
            optionMenus[i].SetActive(false);
        }

        activeOption = optionMenus[index];
        activeOption.SetActive(true);
    }

    public void OpenSubMenu(int index){
        //note:options = 0, save = 1, confirmation = 2
        activeSubMenu = subMenus[index];
        activeSubMenu.SetActive(true);       
    }

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
        }
    }

    //same as close but bound to a button
    public void Back(){

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
