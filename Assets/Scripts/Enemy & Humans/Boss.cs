using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private float dmgCooldown = 3f;
    private bool attackable = true;
    private int phase = 0;
    private int hits = 0;

    [SerializeField] private GameObject[] phases;
    [SerializeField] private GameObject initial;
    [SerializeField] private AnimationClip[] introAnimations;
    private PlayerMovement player;

    private Tendrils[] tendrils;
    public Transform playerOrigin;

    public void Reset(){
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        StopCoroutine("StartFight");
        phase = 0;
        hits = 0;
        attackable = true;

        initial.SetActive(true);

        for(int i = 0; i < phases.Length; i++){
            phases[i].SetActive(false);
        }

        StartCoroutine("StartFight");
    }

    public IEnumerator StartFight(){

        player.enabled = false;
        initial.SetActive(false);
        phases[0].SetActive(true);   

        tendrils = phases[0].GetComponentsInChildren<Tendrils>(); 

        AudioManager.Instance.PlayMusic("boss");
        yield return new WaitForSeconds(introAnimations[0].length); 
        
        for(int i = 0; i < tendrils.Length; i++){
            tendrils[i].Reset();
            StartCoroutine(tendrils[i].Strike());
        }

        player.enabled = true;
    }

    public IEnumerator OnHit(){

        
        if(attackable && hits < 3){
            hits++;
            player.transform.position = playerOrigin.position;

            attackable = false;        
            AudioManager.Instance.PlaySFXAtPoint("boss_ouch", this.transform);

            anim.SetBool("iframe", true);
            yield return new WaitForSeconds(dmgCooldown);
            anim.SetBool("iframe", false);            
            
            //phase change
            phase++;

            for(int i = 0; i < tendrils.Length; i++){
                tendrils[i].Reset();
            }

            phases[phase].SetActive(true);

            for(int i = 0; i < phases.Length; i++){
                if(i != phase){
                    phases[i].SetActive(false);
                }
            }

            tendrils = phases[phase].GetComponentsInChildren<Tendrils>(); 

 
            yield return new WaitForSeconds(introAnimations[phase].length);

            for(int i = 0; i < tendrils.Length; i++){
                StartCoroutine(tendrils[i].Strike());
            }

            attackable = true;    

        } else if(hits >= 3){
            OnDeath();
        }
    }

    private void OnDeath(){
        Debug.Log("died");
        //play death animation
        //destroy object
        //you are win

        //replace with end dialogue then load next scene
        SceneManager.LoadScene(0);
    }

}

