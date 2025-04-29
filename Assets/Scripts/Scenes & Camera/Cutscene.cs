using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Cutscene : MonoBehaviour
{
    private PlayerControls ctrls;
    public DialogueSystem di;
    public string key;
    public PlayerInteractions interactions;
    public PlayerMovement movement;
    public bool isEnd = false;
    public GameObject credits;
    public GameObject textContainer;

    void Awake(){
        movement.enabled = false;
        interactions.enabled = false;
        ctrls = new PlayerControls();
    }

    void OnEnable(){
        ctrls.Land.Pickup.performed += AdvanceDialogue;
        ctrls.Enable();        
        Cursor.visible = false;
        di.cutsceneText = textContainer;
        di.SetCutsceneText(key);
    }

    void OnDisable(){
        ctrls.Land.Pickup.performed -= AdvanceDialogue;
        ctrls.Disable();
    }

    private void AdvanceDialogue(InputAction.CallbackContext ctx){

        if(ctx.performed){

            if(di.cutsceneText.GetComponent<TypewriterEffect>().done){
                int index = di.FindIndexByKey(key);

                if(di.diList[index].hasNext){
                    di.SetCutsceneText(key);
                } else{
                    if(!isEnd){
                        movement.enabled = true;
                        interactions.enabled = true;
                        AudioManager.Instance.PlayMusic("ambient");
                        this.gameObject.SetActive(false);                        
                    }else{
                        credits.SetActive(true);
                    }
                }                 
            }
        }
    }
}
