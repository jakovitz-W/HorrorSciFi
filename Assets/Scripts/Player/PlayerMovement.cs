using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    public float baseSpeed = 30f;
    public float runSpeed = 30f;
    private PlayerControls playerControls;
    private LevelManager levelManager;
    [HideInInspector] public float horizontalMove, verticalMove;

    [SerializeField] private GameObject tinyDrone;
    [SerializeField] private GameObject droneCam;
    public bool droneActive;
    public bool droneUnlocked;

    private void Awake(){
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        playerControls = new PlayerControls();
        tinyDrone.SetActive(false);
        droneCam.SetActive(false);
        droneActive = false;
        droneUnlocked = false;
        runSpeed = baseSpeed;
    }

    private void OnEnable(){
        playerControls.Land.ActivateDrone.performed += SwapToDrone;
        playerControls.Enable();
    }

    private void OnDisable(){
        playerControls.Land.ActivateDrone.performed -= SwapToDrone;
        playerControls.Disable();
    }

    void SwapToDrone(InputAction.CallbackContext ctx){

        if(ctx.performed){
            if(!droneActive && droneUnlocked){
                droneActive = true;
                tinyDrone.SetActive(true);
                tinyDrone.transform.position = transform.position;
                droneCam.SetActive(true);
                droneCam.transform.position = tinyDrone.transform.position;
                
                horizontalMove = 0;
                verticalMove = 0;
            }
        }
    }

    void FixedUpdate(){

        if(!droneActive){ //so player doesn't keep moving after activating tiny dude

            horizontalMove = playerControls.Land.Move.ReadValue<Vector2>().x * runSpeed;
            verticalMove = playerControls.Land.Move.ReadValue<Vector2>().y * runSpeed;
        } else{
            horizontalMove = 0;
            verticalMove = 0;
        }
        controller.Move(horizontalMove * Time.fixedDeltaTime, verticalMove * Time.fixedDeltaTime);
    }
    public void OnDeath(){

        GameObject checkpoint = levelManager.levels[levelManager.LIndex].checkpoint;
        transform.position = checkpoint.transform.position;
        horizontalMove = 0f;
    }

    public void ReduceSpeed(){
        if(runSpeed != 0){
            runSpeed -= (.05f * runSpeed);
        } else{
            //too slow to continue
            OnDeath();
            ResetSpeed();
        }
    }

    public void ResetSpeed(){
        runSpeed = 40f;
    }

    void OnTriggerEnter2D(Collider2D other){

        if(other.gameObject.tag == "BlockTrigger"){ //activates a barrier within the scene

            //play blocking animation
            GameObject obstacle = other.transform.parent.gameObject;
            obstacle.transform.Find("Sprite").gameObject.GetComponent<Animator>().Play("grate-drop");
            //obstacle.GetComponent<SpriteRenderer>().enabled = true;
            obstacle.GetComponent<Collider2D>().enabled = true;
            
            other.gameObject.SetActive(false); //make sure player doesn't get stuck a second time
        }
    }

    void OnCollisionEnter2D(Collision2D col){

        if(col.gameObject.tag == "Enemy"){
            ReduceSpeed();
        }
    }
}
