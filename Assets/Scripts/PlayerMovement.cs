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
        playerControls.Enable();
    }

    private void OnDisable(){
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

    //change to pickup (callback context with pickup)
    void OnTriggerEnter2D(Collider2D other){
        
        if(other.gameObject.CompareTag("Key Item")){
            levelManager.ItemCollected(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
}
