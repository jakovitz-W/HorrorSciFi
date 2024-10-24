using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour
{
    public LevelManager levelManager;
    private Level currentLevel;
    private PlayerControls playerControls;
    private Rigidbody2D rb;
    private List<GameObject> humans;
    public int humansSaved = 0;
    private bool hasTorch, hasTaser;
    public GameObject[] keyUI;
    private int keyNum; //keeps track of how many keys the player has picked up
    public float radius = 1f;

    void Awake(){
        
        playerControls = new PlayerControls();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        rb = GetComponent<Rigidbody2D>();
        humans = new List<GameObject>();
        hasTorch = false;
        hasTaser = false;
        currentLevel = levelManager.levels[0];
        keyNum = 0;
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

            Collider2D col = Physics2D.OverlapCircle(transform.position, radius, mask);

            if(col != null){

                GameObject target = col.gameObject;

                if(target.tag == "Key Item"){

                    currentLevel.hasKey = true;
                    keyUI[keyNum].GetComponent<Image>().color = Color.white;
                    keyNum++;
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

            Collider2D col = Physics2D.OverlapCircle(transform.position, radius, mask);

            if(col != null){
                GameObject target = col.gameObject;

                //need to account for edge cases where there's multiple objects in the overlap circle
                //can be done with a foreach loop
                switch(target.tag){
                    case "Key Item":
                        //TODO: dialogue box
                        break;
                    case "Door":
                        if(CheckDoor(target)){
                            target.GetComponent<DoorScript>().SetUnlocked();
                        }
                        break;
                    case "Checkpoint":
                        StartCoroutine(levelManager.Backtrack());
                        int i = levelManager.LIndex - 1;
                        currentLevel = levelManager.levels[i]; //proboably a more elegant way to do this
                        break;
                    case "Human":
                        //TODO: check if human has special function
                        humans.Add(target);
                        break;
                    case "Dropoff":
                        humansSaved += humans.Count;
                        humans.Clear();
                        break;
                    case "Meltable":
                        if(hasTorch){
                            //TODO: play melting animation
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

    //clean up this function, maybe utilize hasKey instead of repeating lines
    bool CheckDoor(GameObject door){

        int i = levelManager.LIndex;

        if(door.GetComponent<DoorScript>().requireKey){

            if(currentLevel.hasKey){

                i++;
                currentLevel = levelManager.levels[i];
                StartCoroutine(levelManager.OnRoomChange(i));
                //remove key requirement to avoid redundancy
                door.GetComponent<DoorScript>().requireKey = false;
                //Debug.Log("Door Unlocked");
                return true;

            } else{
                
                //TODO: popup text box saying door is locked, need to find key, etc
                Debug.Log("This door is locked");
                return false;
            }
        } else{

            i++;
            currentLevel = levelManager.levels[i];
            StartCoroutine(levelManager.OnRoomChange(i));
            return false; //returning false because door should already appear unlocked
        }
    }
}
