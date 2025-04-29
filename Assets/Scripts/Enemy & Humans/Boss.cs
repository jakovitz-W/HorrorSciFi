using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private float dmgCooldown = 3f;
    private bool attackable = true;
    private int phase = 0;
    private int hits = 0;

    public GameObject[] phases;
    [SerializeField] private AnimationClip[] introAnimations;
    private PlayerMovement player;

    private Tendrils[] tendrils;
    public Transform playerOrigin;
    public GameObject endCutscene;

    public void Reset(){
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        StopCoroutine("StartFight");
        phase = 0;
        hits = 0;
        attackable = true;

        phases[0].SetActive(true);   
        for(int i = 1; i < phases.Length; i++){
            phases[i].SetActive(false);
        }

        StartCoroutine("StartFight");
    }

    public IEnumerator StartFight(){

        player.enabled = false;
        
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

        hits++;  
        if(attackable && hits < 3){
            Debug.Log(hits);

            player.transform.position = playerOrigin.position;
            player.enabled = false;
            player.Reset();

            attackable = false;        
            AudioManager.Instance.PlaySFXAtPoint("boss_ouch", this.transform);

            anim.SetBool("iframe", true);
            yield return new WaitForSeconds(dmgCooldown);
            anim.SetBool("iframe", false);            
            
            //phase change
            phase++;

            phases[phase].SetActive(true);

            tendrils = phases[phase].GetComponentsInChildren<Tendrils>(); 

            
            yield return new WaitForSeconds(introAnimations[phase].length);

            player.enabled = true;
            for(int i = 0; i < tendrils.Length; i++){
                StartCoroutine(tendrils[i].Strike());
            }

            attackable = true;    

        } else if(hits >= 3){
            AudioManager.Instance.PlaySFXAtPoint("boss_ouch", this.transform);
            OnDeath();
        }
    }

    private void OnDeath(){
        this.gameObject.SetActive(false);
        endCutscene.SetActive(true);
        AudioManager.Instance.PauseAll();
    }

}

