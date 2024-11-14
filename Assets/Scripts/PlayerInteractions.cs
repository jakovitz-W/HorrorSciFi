using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerInteractions : MonoBehaviour
{
    public LevelManager levelManager;
    private Level currentLevel;
    private PlayerControls playerControls;
    private Rigidbody2D rb;
    private List<GameObject> humans;
    public int humansSaved = 0;

    public GameObject[] keyUI;
    private int keyNum; //keeps track of how many keys the player has picked up
    public float radius = 1f;

    public GameObject flameThrower;
    private bool hasTorch, hasTaser;
    private bool torchActive, taserActive;
    [SerializeField] private TMP_Text activeToolText;


    void Awake(){
        
        playerControls = new PlayerControls();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        rb = GetComponent<Rigidbody2D>();
        humans = new List<GameObject>();

        activeToolText.text = "Active Tool: None";
        hasTorch = false;
        flameThrower.SetActive(false);
        taserActive = false;

        hasTaser = false;
        taserActive = false;

        currentLevel = levelManager.levels[0];
        keyNum = 0;
    }

    private void OnEnable(){
        playerControls.Land.Pickup.performed += Pickup;
        playerControls.Land.Interact.performed += Interact;
        playerControls.Land.UseWeapon.started += UseWeapon;
        playerControls.Land.UseWeapon.canceled += UseWeapon;
        playerControls.Land.SwapWeapon.performed += SwapWeapon;
        playerControls.Enable();
    }

    private void OnDisable(){
        playerControls.Land.Pickup.performed -= Pickup;
        playerControls.Land.Interact.performed -= Interact; 
        playerControls.Land.UseWeapon.started -= UseWeapon;
        playerControls.Land.UseWeapon.canceled -= UseWeapon;
        playerControls.Land.SwapWeapon.performed -= SwapWeapon;
        playerControls.Disable();
    }

    void UseWeapon(InputAction.CallbackContext ctx){

        if(ctx.started){
            if(hasTorch && torchActive){
                //check if adding lights to particle path is possible
                flameThrower.SetActive(true);
            }

            LayerMask mask = LayerMask.GetMask("Interactable");
            Collider2D col = Physics2D.OverlapCircle(transform.position, radius, mask);
            
            if(col != null){
                GameObject target = col.gameObject;

                if(target.tag == "Meltable"){

                    if(hasTorch && torchActive){
                        //StartCoroutine(target.Melt());
                        target.GetComponent<Collider2D>().enabled = false;
                    }
                } else if(target.tag == "Zappable"){

                    if(hasTaser && taserActive){
                        //StartCoroutine(target.Zap())
                        target.GetComponent<Collider2D>().enabled = false; 
                    }
                    
                } else if(target.tag == "Enemy"){

                }
            }
        }

        if(ctx.canceled){
            if(hasTorch && torchActive){
                flameThrower.SetActive(false);
            }
           
        }
    }

    void SwapWeapon(InputAction.CallbackContext ctx){
        
        if(ctx.performed){
            if(torchActive && hasTaser){
                torchActive = false;
                taserActive = true;
                activeToolText.text = "Active Tool: Taser";

            } else if(taserActive && hasTorch){
                torchActive = true;
                taserActive = false;
                activeToolText.text = "Active Tool: Blowtorch";
            }
        }
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
                    torchActive = true;
                    activeToolText.text = "Active Tool: Blowtorch";
                }else if(target.tag == "Taser"){

                    hasTaser = true;
                    taserActive = true;
                    torchActive = false;
                    activeToolText.text = "Active Tool: Taser";
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
                        currentLevel = levelManager.levels[i];
                        break;
                    case "Human":
                        //TODO: check if human has special function
                        humans.Add(target);
                        break;
                    case "Dropoff":
                        humansSaved += humans.Count;
                        humans.Clear();
                        break;
                    case "Button":
                        
                        break;
                    case "Exit":
                        transform.position = levelManager.levels[levelManager.LIndex].checkpoint.transform.position; //oof
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
