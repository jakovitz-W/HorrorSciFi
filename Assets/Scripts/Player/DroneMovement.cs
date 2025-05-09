using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneMovement : MonoBehaviour
{   
    [SerializeField] private CharacterController controller;
    private PlayerControls playerControls;
    [SerializeField] private GameObject player;
    private float horizontalMove, verticalMove;
    [SerializeField] private float speed;
    [SerializeField] private GameObject droneCam;
    [SerializeField] private float interactRadius = .5f;

    void Awake(){
        playerControls = new PlayerControls();
    }

    void OnEnable(){
        playerControls.Land.ActivateDrone.performed += SwapToPlayer;
        playerControls.Land.Interact.performed += Interact;
        playerControls.Enable();
    }

    void OnDisable(){
        playerControls.Land.ActivateDrone.performed -= SwapToPlayer;
        playerControls.Land.Interact.performed -= Interact;
        playerControls.Disable();
    }

    void FixedUpdate(){
        horizontalMove = playerControls.Land.Move.ReadValue<Vector2>().x * speed;
        verticalMove = playerControls.Land.Move.ReadValue<Vector2>().y * speed;
        controller.Move(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.fixedDeltaTime);
    }

    void Interact(InputAction.CallbackContext ctx){

        if(ctx.performed){

            LayerMask mask = LayerMask.GetMask("Interactable");
            Collider2D col = Physics2D.OverlapCircle(transform.position, interactRadius, mask);

            if(col != null){
                GameObject target = col.gameObject;

                if(target.tag == "ControlPanel"){
                    target.GetComponent<ButtonScript>().ActivateAll();
                } else if(target.tag == "reset_button"){
                    target.GetComponent<ResetButton>().Reset();
                }
            }
        }
    }

    void SwapToPlayer(InputAction.CallbackContext ctx){

        if(ctx.performed){

            droneCam.SetActive(false);
            player.GetComponent<PlayerMovement>().droneActive = false;
            horizontalMove = 0;
            verticalMove = 0;

            this.gameObject.SetActive(false);
        }
    }
}
