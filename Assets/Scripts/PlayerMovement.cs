using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private PlayerControls playerControls;
    public LevelManager levelManager;
    private int nextRoom;
    private Rigidbody2D rb;
    public float runSpeed = 40f;
    private float horizontalMove;
    private float verticalMove;
    public List<GameObject> humans;
    public int humansSaved = 0;
    public List<GameObject> keys;
    private bool hasTorch, hasTaser;
    public GameObject[] weapons;

    private void Awake(){
        playerControls = new PlayerControls();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        rb = GetComponent<Rigidbody2D>();
        hasTorch = false;
        hasTaser = false;
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

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "BlockTrigger"){
            //play blocking animation, activate spriterenderer for now
            GameObject obstacle = other.transform.parent.gameObject;
            obstacle.GetComponent<SpriteRenderer>().enabled = true;
            obstacle.GetComponent<Collider2D>().enabled = true;
            other.gameObject.SetActive(false); //make sure player doesn't get stuck a second time
        }
    }

    void Pickup(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Key Item") | LayerMask.GetMask("Weapon");
        if(ctx.performed){
            Collider2D col = Physics2D.OverlapCircle(transform.position, .5f, mask);
            if(col != null){

                GameObject target = col.gameObject;

                if(target.tag == "Key Item"){
                    keys.Add(target);
                    //levelManager.ItemCollected(target);
                } else if(target.tag == "Torch"){
                    //weapons[0] = target;
                    hasTorch = true;
                } else if(target.tag == "Taser"){
                    weapons[1] = target;
                    hasTaser = true;
                }
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
                        break;
                    case "Human":
                        //check if human has special function
                        humans.Add(target);
                        break;
                    case "Door":
                        CheckDoor(target);
                        break;
                    case "Checkpoint":
                        StartCoroutine(levelManager.Backtrack());
                        break;
                    case "Dropoff":
                        humansSaved += humans.Count;
                        humans.Clear();
                        break;
                    case "Meltable":
                        if(hasTorch){
                            //play melting animation
                            target.GetComponent<Collider2D>().enabled = false;
                        }
                        break;
                    case "Zappable":

                        if(hasTaser){
                            //this function is more complex, needs to be seperated
                            //do routine based on type of object zapped
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void CheckDoor(GameObject door){
        bool doorUnlocked = false;
        for(int i = 0; i < keys.Count; i++){
            if(keys[i].GetComponent<ItemScript>().id == levelManager.LIndex){ //assumes only one door per level
                nextRoom = i + 1;
                doorUnlocked = true;
            }
        }

        if(doorUnlocked){
            //play open animation
            //go through door
            StartCoroutine(levelManager.OnRoomChange(nextRoom));
            Debug.Log("Door Unlocked");
        }
        else{
            //popup text box saying door is locked, need to find key, etc
            Debug.Log("This door is locked");
        }
    }
}
