using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    public LevelManager levelManager;
    private Level currentLevel;
    private PlayerControls playerControls;
    private Rigidbody2D rb;
    public List<GameObject> humans;
    public int humansSaved = 0;
    private bool hasTorch, hasTaser;
    

    void Awake(){
        
        playerControls = new PlayerControls();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        rb = GetComponent<Rigidbody2D>();
        hasTorch = false;
        hasTaser = false;
        currentLevel = levelManager.levels[0];
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

    void Pickup(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Key Item", "Weapon");
        if(ctx.performed){

            Collider2D col = Physics2D.OverlapCircle(transform.position, .5f, mask);

            if(col != null){

                GameObject target = col.gameObject;

                if(target.tag == "Key Item"){

                    currentLevel.hasKey = true;
                    //add to UI
                }else if(target.tag == "Torch"){

                    hasTorch = true;
                }else if(target.tag == "Taser"){

                    hasTaser = true;
                }
                target.SetActive(false);
            }
        }
    }

    void Interact(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Interactable", "KeyItem");

        if(ctx.performed){

            Collider2D col = Physics2D.OverlapCircle(transform.position, .5f, mask);

            if(col != null){
                GameObject target = col.gameObject;

                switch(target.tag){

                    case "Key Item":
                        //dialogue box
                        break;

                    case "Door":
                        CheckDoor();
                        break;

                    case "Checkpoint":
                    
                        StartCoroutine(levelManager.Backtrack());
                        int i = levelManager.LIndex - 1;
                        currentLevel = levelManager.levels[i]; //proboably a more elegant way to do this
                        break;

                    case "Human":
                        //check if human has special function
                        humans.Add(target);
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

    void CheckDoor(){

        int i = levelManager.LIndex;

        if(currentLevel.hasKey){
            //play open animation or change door sprite to green light
            i++;
            currentLevel = levelManager.levels[i];
            StartCoroutine(levelManager.OnRoomChange(i));
            Debug.Log("Door Unlocked");
        } else{
            //popup text box saying door is locked, need to find key, etc
            Debug.Log("This door is locked");
        }
    }
}
