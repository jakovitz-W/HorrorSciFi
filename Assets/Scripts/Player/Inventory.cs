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
    [SerializeField] private UpgradeSystem upgradeSystem;
    [SerializeField] private TMP_Text following, tokens;
    [SerializeField] private Image torch, taser;
    [SerializeField] private Image[] collectibles;
    [SerializeField] private GameObject menu;

    void Awake(){
        playerControls = new PlayerControls();
    }

    private void OnEnable(){
        playerControls.General.Inventory.performed += OpenMenu;
    }

    private void OnDisable(){
        playerControls.General.Inventory.performed -= OpenMenu;
    }

    void OpenMenu(InputAction.CallbackContext ctx){
        
        if(ctx.performed){
            if(!menu.activeSelf){ //menu is closed
                menu.SetActive(true);
                following.text = "Following: " + player.humans.Count;
                tokens.text = "Tokens: " + upgradeSystem.wallet;
                //set torch/taser images to active/inactive
                //check collectibles

            } else{ //menu open
                menu.SetActive(false);
            }
        }
    }
}
