using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Secret : MonoBehaviour
{
    public GameObject roomEntry;
    private bool playerInCollider;
    private PlayerControls ctrls;
    private GameObject player;

    void Awake(){
        ctrls = new PlayerControls();
        player = GameObject.FindWithTag("Player");
    }

    void OnEnable(){
        ctrls.General.OpenDebugMenu.performed += EnterRoom;
        ctrls.Enable();
    }

    void OnDisable(){
        ctrls.General.OpenDebugMenu.performed -= EnterRoom;
        ctrls.Disable();
    }

    void EnterRoom(InputAction.CallbackContext ctx){

        if(ctx.performed && playerInCollider){
            player.transform.position = roomEntry.transform.position;
            Cursor.visible = true;            
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "Player"){
            playerInCollider = true;
        }
    }

    void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.tag == "Player"){
            playerInCollider = false;
        }
    }


}
