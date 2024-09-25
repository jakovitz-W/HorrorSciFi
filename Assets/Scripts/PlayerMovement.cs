using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private PlayerControls playerControls;
    public LevelManager levelManager;
    private Rigidbody2D rb;
    public float runSpeed = 40f;
    private float horizontalMove;
    private float verticalMove;
    public bool isInvincible = false;

    private void Awake(){
        playerControls = new PlayerControls();
        //levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void OnEnable(){
        playerControls.Land.Pickup.performed += Pickup;
        playerControls.Land.Interact.performed += Interact;
        playerControls.Enable();
    }

    private void OnDisable(){
        playerControls.Land.Pickup.performed -= Pickup;
        playerControls.Land.Interact.performed -= Interact; 
        playerControls.Disable();
    }

    void FixedUpdate(){
        horizontalMove = playerControls.Land.Move.ReadValue<Vector2>().x * runSpeed;
        verticalMove = playerControls.Land.Move.ReadValue<Vector2>().y * runSpeed;
        
    }
    void Update(){
        controller.Move(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.fixedDeltaTime);
    }

    public void OnDeath(){

        transform.position = levelManager.GetCurrentCheckpoint().transform.position;
        horizontalMove = 0f;

    }

    public void ReduceSpeed(){
        if(runSpeed != 0){
            runSpeed -= (.005f * runSpeed);
        } else{
            //too slow to continue
            OnDeath();
            ResetSpeed();
        }
    }

    public void ResetSpeed(){
        runSpeed = 40f;
    }

    void Pickup(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Key Item");
        if(ctx.performed){
            Collider2D col = Physics2D.OverlapCircle(transform.position, .5f, mask);
            if(col != null){
                Debug.Log("Pickup");
                GameObject target = col.gameObject;
                //levelManager.ItemCollected(target);
                Destroy(target);
            }
        }
    }

    void Interact(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Interactable") | LayerMask.GetMask("Key Item");
        if(ctx.performed){
            Collider2D col = Physics2D.OverlapCircle(transform.position, .5f, mask);
            if(col != null){
                GameObject target = col.gameObject;

                switch(target.tag){
                    case "Key Item":
                        Debug.Log("key");
                        break;
                    case "Human":
                        break;
                    case "Door":
                        break;
                    case "Dropoff":
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
