using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*Keep Disabled until Boss Room*/
public class BossCombat : MonoBehaviour
{
    private PlayerInteractions interactions;
    [SerializeField] private LevelManager lm;
    private PlayerControls ctrls;
    [SerializeField] private int hitpoints = 3;
    private bool invincible = false;
    [SerializeField] private float iTime = 0.5f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private Boss boss;

    void OnEnable(){
        interactions = GetComponent<PlayerInteractions>();
        interactions.enabled = false;
        ctrls = new PlayerControls();
        lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        StartCoroutine(boss.StartFight());
        ctrls.Land.UseWeapon.performed += UseWeapon;
    }

    void OnDisable(){
        ctrls.Land.UseWeapon.performed -= UseWeapon;
    }

    void UseWeapon(InputAction.CallbackContext ctx){

        LayerMask mask = LayerMask.GetMask("Boss");
        if(ctx.performed){
            Collider2D col = Physics2D.OverlapCircle(transform.position, attackRadius, mask);

            StartCoroutine(col.gameObject.GetComponent<Boss>().OnHit());
        }
    }

    void OnCollisionEnter2D(Collision2D col){

        if(col.gameObject.tag == "Boss" && !invincible){

            StartCoroutine("IFrames");
            hitpoints--;

            if(hitpoints <= 0){
                OnDeath();
                invincible = false;
                StopCoroutine("IFrames");
            }

        }
    }

    private IEnumerator IFrames(){
        invincible = true;
        //flashing opacity animation
        yield return new WaitForSeconds(iTime);
        //return to full opacity
        invincible = false;
    }

    void OnDeath(){
        interactions.enabled = true;
        //StartCoroutine(lm.Backtrack());
    }
}
