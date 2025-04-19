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
    public List<GameObject> humans;

    public GameObject[] keyUI;
    private int keyNum; //keeps track of how many keys the player has picked up
    public float attackRadius = 1.5f;
    [SerializeField] private float interactRadius = 1.5f;
    public float stunTime = 3f;

    public GameObject flameThrower, taser;
    public bool hasTorch, hasTaser;
    public bool torchActive, taserActive;
    [SerializeField] private GameObject[] toolsUI;
    
    [SerializeField] private DialogueSystem diSystem;
    [SerializeField] private UpgradeSystem upgradeSystem;
    private PlayerMovement pMovement;
    private BossCombat bossCombat;


    public bool hasCure = false;

    void Awake(){
        
        playerControls = new PlayerControls();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        pMovement = GetComponent<PlayerMovement>();
        bossCombat = GetComponent<BossCombat>();
        rb = GetComponent<Rigidbody2D>();
        humans = new List<GameObject>();

        bossCombat.enabled = false;
        hasTorch = false;
        flameThrower.SetActive(false);
        taserActive = false;
        taser.SetActive(false);

        hasTaser = false;
        taserActive = false;

        //get from save system
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
            diSystem.SetText("Move", true, false, -1);

        } else if(col.gameObject.name == "InteractHint"){
            col.gameObject.SetActive(false);
            diSystem.SetText("Interact", true, false, -1);

        } else if(col.gameObject.name == "PickupHint"){
            col.gameObject.SetActive(false);
            diSystem.SetText("Pickup", true, false, -1);
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
                Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, attackRadius, mask);
                
                if(col.Length != 0){

                    for(int i = 0; i < col.Length; i++){
                        GameObject target = col[i].gameObject;

                        if(target.tag == "Meltable"){

                            if(hasTorch && torchActive){
                                StartCoroutine(target.GetComponent<GrateController>().Melt());
                            }
                        } else if(target.tag == "ControlPanel"){

                            if(hasTaser && taserActive){

                                target.GetComponent<ButtonScript>().ActivateAll();                                
                            }
                            
                        } else if(target.tag == "Enemy"){

                            if(hasTorch || hasTaser){
                                StartCoroutine(target.GetComponent<EnemyBehavior>().Stun(stunTime));
                            }
                            
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
                toolsUI[0].SetActive(false);
                toolsUI[1].SetActive(true);
                taserActive = true;


            } else if(taserActive && hasTorch){
                torchActive = true;
                taserActive = false;
                toolsUI[1].SetActive(false);
                toolsUI[0].SetActive(true);
                taser.SetActive(false);

            } else if((!torchActive && !taserActive) && (hasTorch && hasTaser)){ //for case when player respawns outside the boss room
                torchActive = true;
                taserActive = false;
                toolsUI[1].SetActive(false);
                toolsUI[0].SetActive(true);
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
                    toolsUI[0].SetActive(true);

                    diSystem.SetText("UseTool", true, false, -1);

                }else if(target.tag == "Taser"){

                    hasTaser = true;
                    taserActive = true;
                    torchActive = false;

                    toolsUI[0].SetActive(false);
                    toolsUI[1].SetActive(true);
                    diSystem.SetText("SwapTool", true, false, -1);
                }
                target.SetActive(false);
            }
        }
    }

    void Interact(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Interactable", "Key Item", "Enemy");

        if(ctx.performed){

            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, interactRadius, mask);
            
            foreach(Collider2D col in cols){
                GameObject target = col.gameObject;
                
                if(!diSystem.diContainer.activeSelf){ //only allow interaction when the dialogue box is not active

                    if(col != null){
                        
                        switch(target.tag){
                            case "Key Item":
                                diSystem.SetText("KeyItem", false, false, -1);
                                break;
                            case "Door":
                                if(CheckDoor(target, true)){
                                    target.GetComponent<DoorScript>().SetUnlocked();
                                }
                                break;
                            case "Meltable":
                                diSystem.SetText("Melt", false, false, -1);
                                break;
                            case "ControlPanel":
                                diSystem.SetText("Zap", false, false, -1);
                                break;
                            case "Checkpoint":
                                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                                rb.velocity = new Vector2(0,0);
                                StartCoroutine(levelManager.Backtrack());
                                int i = levelManager.LIndex - 1;
                                currentLevel = levelManager.levels[i];
                                break;
                            case "Human":
                                //TODO: check if human has special function
                                //get human dialogue box if it has one
                                HumanBehavior attributes = target.GetComponent<HumanBehavior>();

                                if(attributes.type != "special"){                                
                                    diSystem.SetText(attributes.dialogueKey, false, true, -1);
                                    if(humans.IndexOf(target) == -1){
                                        humans.Add(target);
                                        attributes.isFollowing = true;
                                    }
                                } else if(attributes.dialogueKey == "FirstMate"){
                                    if(!hasCure){
                                        diSystem.SetText(attributes.dialogueKey, false, false, 0);
                                    } else{
                                        currentLevel.hasKey = true;
                                        keyUI[keyNum].GetComponent<Image>().color = Color.white;
                                        keyNum++;
                                        diSystem.SetText(attributes.dialogueKey, false, false, 1);
                                    }
                                }
                                break;
                            case "Exit":
                                transform.position = levelManager.levels[levelManager.LIndex].checkpoint.transform.position; //oof
                                break;
                            case "Enemy":
                                diSystem.SetText("Monster", false, false, -1);
                                break;
                            case "Upgrade":
                                upgradeSystem.OpenMenu();
                                break;
                            default:
                                break;
                        }
                    }
                }else{
                    if(target.tag == "Door"){ //let player through doors while speaking to astronaut/creatures
                        if(CheckDoor(target, false)){
                            target.GetComponent<DoorScript>().SetUnlocked();
                        }
                    }
                }
            } 
        }
    }

    public void RemoveHuman(GameObject human){
        upgradeSystem.humansSaved++;
        humans.Remove(human);
    }

    public void TeleportHumans(int level){
        for(int i = 0; i < humans.Count; i++){
            humans[i].transform.position = transform.position;
            humans[i].transform.parent = levelManager.levelParents[level].transform;
            humans[i].GetComponent<HumanBehavior>().Unlink();
        }
    }

    bool CheckDoor(GameObject door, bool showDialogue){

        int i = levelManager.LIndex;

        if(door.GetComponent<DoorScript>().requireKey){

            if(door.GetComponent<DoorScript>().isBoss){
                if(hasCure){
                    i++;
                    currentLevel = levelManager.levels[i];
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    rb.velocity = new Vector2(0,0);
                    StartCoroutine(levelManager.OnRoomChange(i));
                    door.GetComponent<DoorScript>().requireKey = false;

                    taserActive = false;
                    torchActive = false;

                    toolsUI[0].SetActive(false);
                    toolsUI[1].SetActive(false);
                    toolsUI[2].SetActive(true);

                    bossCombat.enabled = true;
                    
                    return true;
                } else{
                    if(showDialogue){
                        diSystem.SetText("MissingCure", false, true, -1);
                        Debug.Log("Requires Cure Item");    
                    }

                    return false;
                }
            } else if(currentLevel.hasKey){

                i++;
                currentLevel = levelManager.levels[i];
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                rb.velocity = new Vector2(0,0);
                StartCoroutine(levelManager.OnRoomChange(i));
                //remove key requirement to avoid redundancy
                door.GetComponent<DoorScript>().requireKey = false;
                //Debug.Log("Door Unlocked");
                return true;

            } else{
                if(showDialogue){
                   diSystem.SetText("DoorLocked", false, true, -1); 
                }
                
                return false;
            }
        } else{

            i++;
            currentLevel = levelManager.levels[i];
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            rb.velocity = new Vector2(0,0);
            StartCoroutine(levelManager.OnRoomChange(i));
            return false; //returning false because door should already appear unlocked
        }
    }
}
