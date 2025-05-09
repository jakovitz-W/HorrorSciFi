using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [HideInInspector] public PlayerControls playerControls;
    [SerializeField] private PlayerInteractions player;
    [SerializeField] private PlayerMovement pMovement;
    [SerializeField] private UpgradeSystem upgradeSystem;
    [SerializeField] private TMP_Text following, tokens;
    [SerializeField] private RawImage torch, taser, cure, drone;
    [SerializeField] private GameObject menu;
    private bool menuActive = false;

    void Awake(){
        playerControls = new PlayerControls();
    }

    //remember to enable playercontrols
    private void OnEnable(){
        playerControls.General.OpenInv.performed += OpenInv;
        playerControls.Enable();
    }

    private void OnDisable(){
        playerControls.General.OpenInv.performed -= OpenInv;
        playerControls.Disable();
    }


    private void OpenInv(InputAction.CallbackContext ctx){
        
        if(ctx.performed){

            menuActive = !menuActive;
            menu.SetActive(menuActive);

            if(menuActive){
                following.text = "Following: " + player.humans.Count;
                tokens.text = "Upgrade Tokens: " + upgradeSystem.wallet;

                if(player.hasTorch){
                    torch.color = new Color(1f, 1f, 1f, 1f); //changing color and alpha value
                }
                if(player.hasTaser){
                    taser.color = new Color(1f, 1f, 1f, 1f);
                }
                if(player.hasCure){
                    cure.color = new Color(1f, 1f, 1f, 1f);
                }

                if(pMovement.droneUnlocked){
                    drone.color = new Color(1f, 1f, 1f, 1f);
                }
            }

        }
    }
}
