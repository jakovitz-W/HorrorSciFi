using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/*TODO: fix bug where player keeps moving on scene change, fix bug where player sometimes moves with mini drone*/
public class PlayerInteractions : MonoBehaviour
{
    public LevelManager levelManager;
    private Level currentLevel;
    [HideInInspector] public PlayerControls playerControls;
    private Rigidbody2D rb;
    [SerializeField] private List<GameObject> humans;
    public int humansSaved = 0;

    public GameObject[] keyUI;
    private int keyNum; //keeps track of how many keys the player has picked up
    public float attackRadius = 1.5f;
    [SerializeField] private float interactRadius = 1.5f;
    public float stunTime = 3f;

    public GameObject flameThrower, taser;
    public bool hasTorch, hasTaser;
    public bool torchActive, taserActive;
    [SerializeField] private TMP_Text activeToolText;
    [SerializeField] private DialogueSystem diSystem;
    [SerializeField] private UpgradeSystem upgradeSystem;

    private PlayerMovement pMovement;


    void Awake(){
        
        playerControls = new PlayerControls();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        pMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        humans = new List<GameObject>();

        activeToolText.text = "Active Tool: None";
        hasTorch = false;
        flameThrower.SetActive(false);
        taserActive = false;
        taser.SetActive(false);

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

    void OnTriggerEnter2D(Collider2D col){

        if(col.gameObject.name == "MoveHint"){
            col.gameObject.SetActive(false);
            diSystem.SetText("Move", true, false);

        } else if(col.gameObject.name == "InteractHint"){
            col.gameObject.SetActive(false);
            diSystem.SetText("Interact", true, false);

        } else if(col.gameObject.name == "PickupHint"){
            col.gameObject.SetActive(false);
            diSystem.SetText("Pickup", true, false);
        }
    }

    void UseWeapon(InputAction.CallbackContext ctx){

        if(ctx.started){
            if(!pMovement.droneActive){
                
                if(hasTorch && torchActive){
                    //check if adding lights to particle path is possible
                    flameThrower.SetActive(true);
                } else if(hasTaser && taserActive){
                    taser.SetActive(true);
                }

                LayerMask mask = LayerMask.GetMask("Interactable", "Enemy");
                Collider2D col = Physics2D.OverlapCircle(transform.position, attackRadius, mask);
                
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

                        if(hasTorch || hasTaser){
                            StartCoroutine(target.GetComponent<EnemyBehavior>().Stun(stunTime));
                        }
                        
                    }
                }
            }
        }

        if(ctx.canceled){
            if(hasTorch && torchActive){
                flameThrower.SetActive(false);
            } else if(hasTaser && taserActive){
                taser.SetActive(false);
            }
           
        }
    }

    void SwapWeapon(InputAction.CallbackContext ctx){
        
        if(ctx.performed){
            if(torchActive && hasTaser){
                torchActive = false;
                flameThrower.SetActive(false); //in case player is still holding down click
                taserActive = true;
                activeToolText.text = "Active Tool: Taser";

            } else if(taserActive && hasTorch){
                torchActive = true;
                taserActive = false;
                taser.SetActive(false);
                activeToolText.text = "Active Tool: Blowtorch";
            }
        }
    }

    void Pickup(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Key Item", "Weapon");
        if(ctx.performed){

            Collider2D col = Physics2D.OverlapCircle(transform.position, interactRadius, mask);

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
                    diSystem.SetText("UseTool", true, false);

                }else if(target.tag == "Taser"){

                    hasTaser = true;
                    taserActive = true;
                    torchActive = false;
                    activeToolText.text = "Active Tool: Taser";
                    diSystem.SetText("SwapTool", true, false);
                }
                target.SetActive(false);
            }
        }
    }

    void Interact(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Interactable", "Key Item", "Enemy");

        if(ctx.performed){

            Collider2D col = Physics2D.OverlapCircle(transform.position, interactRadius, mask);
            
            if(!diSystem.diContainer.activeSelf){ //only allow interaction when the dialogue box is not active

                if(col != null){
                    GameObject target = col.gameObject;

                    //need to account for edge cases where there's multiple objects in the overlap circle
                    //can be done with a foreach loop
                    switch(target.tag){
                        case "Key Item":
                            diSystem.SetText("KeyItem", false, false);
                            break;
                        case "Door":
                            if(CheckDoor(target)){
                                target.GetComponent<DoorScript>().SetUnlocked();
                            }
                            break;
                        case "Meltable":
                            diSystem.SetText("Melt", false, false);
                            break;
                        case "Zappable":
                            diSystem.SetText("Zap", false, false);
                            break;
                        case "Checkpoint":
                            StartCoroutine(levelManager.Backtrack());
                            int i = levelManager.LIndex - 1;
                            currentLevel = levelManager.levels[i];
                            break;
                        case "Human":
                            //TODO: check if human has special function
                            //get human dialogue box if it has one
                            HumanBehavior attributes = target.GetComponent<HumanBehavior>();
                            diSystem.SetText(attributes.dialogueKey, false, true);

                            if(humans.IndexOf(target) == -1){
                                humans.Add(target);
                                attributes.isFollowing = true;
                            }

                            break;
                        case "Button":
                            
                            break;
                        case "Exit":
                            transform.position = levelManager.levels[levelManager.LIndex].checkpoint.transform.position; //oof
                            break;
                        case "Enemy":
                            diSystem.SetText("Monster", false, false);
                            break;
                        case "Upgrade":
                            upgradeSystem.OpenMenu();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void RemoveHuman(GameObject human){
        humansSaved++;
        humans.Remove(human);
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
                
                diSystem.SetText("DoorLocked", false, true);
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
