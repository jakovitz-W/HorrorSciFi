using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float runSpeed = 40f;
    private PlayerControls playerControls;
    private LevelManager levelManager;
    private float horizontalMove;
    private float verticalMove;

    private void Awake(){
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        playerControls = new PlayerControls();
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

        GameObject checkpoint = levelManager.levels[levelManager.LIndex].checkpoint;
        transform.position = checkpoint.transform.position;
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

    void OnTriggerEnter2D(Collider2D other){

        if(other.gameObject.tag == "BlockTrigger"){ //activates a barrier within the scene

            //play blocking animation, activate spriterenderer for now
            GameObject obstacle = other.transform.parent.gameObject;
            obstacle.GetComponent<SpriteRenderer>().enabled = true;
            obstacle.GetComponent<Collider2D>().enabled = true;
            
            other.gameObject.SetActive(false); //make sure player doesn't get stuck a second time
        }
    }
}
