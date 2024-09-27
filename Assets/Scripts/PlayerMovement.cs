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
    public List<GameObject> humans;
    public int humansSaved = 0;
    public List<GameObject> keys;
    
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
                GameObject target = col.gameObject;
                keys.Add(target);
                //levelManager.ItemCollected(target);
                target.SetActive(false);
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
                        Debug.Log("Human");
                        humans.Add(target);
                        break;
                    case "Door":
                        Debug.Log("Door");
                        CheckDoor(target);
                        break;
                    case "Dropoff":
                        Debug.Log("Dropoff");
                        humansSaved += humans.Count;
                        humans.Clear();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void CheckDoor(GameObject door){
        for(int i = 0; i < keys.Count; i++){
            if(keys[i].GetComponent<ItemScript>().id == 0 /*levelManager.doors.IndexOf(door)*/){ //yikes
                door.GetComponent<Collider2D>().enabled = false;
                Debug.Log("Door Unlocked");
            }
        }
    }
}
