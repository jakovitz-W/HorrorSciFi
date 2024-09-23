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
    private bool isCrouched = false;
    private bool isJumping = false;
    private bool isSprinting = false;
    private float jumpCount = 0f;
    public bool ignoreGround = false;
    public bool isInvincible = false;

    private void Awake(){
        playerControls = new PlayerControls();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void OnEnable(){
        playerControls.Enable();
    }

    private void OnDisable(){
        playerControls.Disable();
    }

    //double jump logic does not work with new unity input system
    void FixedUpdate(){
        horizontalMove = playerControls.Land.Move.ReadValue<Vector2>().x * runSpeed;
        //Debug.Log(horizontalMove);
        if(playerControls.Land.Jump.ReadValue<float>() > 0 && jumpCount < 1){
            //Debug.Log(playerControls.Land.Jump.ReadValue<float>());
            isJumping = true;
            jumpCount++;
        }
        if(playerControls.Land.Crouch.ReadValue<float>() > 0){
            isCrouched = true;
        }else{
            isCrouched = false;
        }
        if(playerControls.Land.Sprint.ReadValue<float>() > 0){
            isSprinting = true;
        } else{
            isSprinting = false;
        }

        if(controller.m_Grounded)
            jumpCount = 0;
        
    }
    void Update(){
        controller.Move(horizontalMove * Time.fixedDeltaTime, isCrouched, isJumping, isSprinting, ignoreGround);
        isJumping = false;    

        if(isInvincible && controller.m_Grounded){
            isInvincible = false;
        }  
    }

    public void OnDeath(bool outOfBounds){
        if(!isInvincible){
            transform.position = levelManager.GetCurrentCheckpoint().transform.position;
            horizontalMove = 0f;
        } else if(outOfBounds){
            transform.position = levelManager.GetCurrentCheckpoint().transform.position;
            horizontalMove = 0f;           
        }
    }

    public IEnumerator OnFinish(){
        yield return new WaitForSeconds(3);
        //GameObject.Find("SceneManager").GetComponent<SceneManagement>().OnMain();
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Coin")){
            levelManager.CoinCollected(other.gameObject);
            other.gameObject.SetActive(false);
        } else if(other.gameObject.CompareTag("DeathTrigger")){
            OnDeath(true);
        }
    }
}
