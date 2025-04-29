using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*Keep Disabled until Boss Room*/
public class BossCombat : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Camera mainCam;
    private PlayerInteractions interactions;
    private PlayerMovement movement;
    [SerializeField] private LevelManager lm;
    private PlayerControls ctrls;
    [SerializeField] private int hitpoints = 3;
    private bool invincible = false;
    [SerializeField] private float iTime = 0.5f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private Boss boss;
    
    void Awake(){
        ctrls = new PlayerControls();
    }
    void OnEnable(){

        mainCam.GetComponent<Animator>().SetBool("boss", true);
        interactions = GetComponent<PlayerInteractions>();
        movement = GetComponent<PlayerMovement>();
        anim =  GetComponent<Animator>();
        interactions.enabled = false;
        lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        ctrls.Land.UseWeapon.performed += UseWeapon;
        ctrls.Enable();  //always remember to enable controls
    }

    void OnDisable(){
        ctrls.Land.UseWeapon.performed -= UseWeapon;
        ctrls.Disable();
    }

    void UseWeapon(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Boss");
        if(ctx.performed){
            Collider2D col = Physics2D.OverlapCircle(transform.position, attackRadius, mask);

            if(col != null){
                if(col.gameObject.tag == "boss_center"){
                    StartCoroutine(boss.OnHit());
                } 
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col){

        if(col.gameObject.tag == "Boss" && !invincible){

            StartCoroutine(IFrames());
            hitpoints--;

            transform.position = boss.playerOrigin.position;
            if(hitpoints <= 0){
                OnDeath();
                invincible = false;
            }

        }
    }

    private IEnumerator IFrames(){

        invincible = true;
        anim.SetBool("iframe", true);
        yield return new WaitForSeconds(iTime);
        anim.SetBool("iframe", false);
        invincible = false;
    }

    void OnDeath(){
        boss.phases[1].SetActive(false);
        boss.phases[2].SetActive(false);
        anim.SetBool("iframe", false);
        invincible = false;
        hitpoints = 3;
        interactions.enabled = true;
        mainCam.GetComponent<Animator>().SetBool("boss", false);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        AudioManager.Instance.StopAll();
        StartCoroutine(lm.Backtrack());
        AudioManager.Instance.PlayMusic("ambient");
        this.enabled = false;
    }
}
