using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private GameObject[] tendrils;
    [SerializeField] private float atkCooldown = 5f;
    [SerializeField] private float speedIncr;
    [SerializeField] private float dmgCooldown = 1f;
    private bool isAttacking = false;
    private bool attackable = true;
    private int phase = 1;
    private int hits = 0;

    /*test vars*/
    public bool testStrike = false;
    public bool startBoss = false;


    public void Reset(){
        anim = GetComponent<Animator>();
        StopCoroutine("StartFight");
        phase = 1;
        hits = 0;
        attackable = true;

        for(int i = 0; i < tendrils.Length; i++){
            tendrils[i].GetComponent<Tendrils>().Reset();
        }
        StartCoroutine("StartFight");
    }

    public IEnumerator StartFight(){
        
        AudioManager.Instance.PlayMusic("boss");
        //start first section of boss theme
        //intro dialogue
        //yield return new WaitForSeconds(1f);
        StartCoroutine("AttackSequence");
        
        for(int i = 0; i < tendrils.Length; i++){
            tendrils[i].GetComponent<Tendrils>().shouldMove = true;
        }
        
        yield return null; //delete later
    }

    void FixedUpdate(){

        /*delete later*/
        if(testStrike){
            testStrike = false;
            for(int i = 0; i < tendrils.Length; i++){
                StartCoroutine(tendrils[i].GetComponent<Tendrils>().Strike());
            }
        }

        /*delete later*/
        if(startBoss){
            startBoss = false;
            StartCoroutine("StartFight");
        }
    }

    private IEnumerator AttackSequence(){

        isAttacking = true;
        float cd = Random.Range(3f, atkCooldown);
        for(int i = 0; i < tendrils.Length; i++){
            StartCoroutine(tendrils[i].GetComponent<Tendrils>().Strike());
        }
        yield return new WaitForSeconds(cd);
        isAttacking = false;
        StartCoroutine(AttackSequence());
    }

    public IEnumerator OnHit(){


        if(!attackable){
            yield return null;
        } else if(hits <= 3){
            hits++;
        } else if(hits > 3){
            OnDeath();
        }
        attackable = false;        
        AudioManager.Instance.PlaySFXAtPoint("boss_ouch", this.transform);
        PhaseChange();
        anim.SetBool("iframe", true);
        yield return new WaitForSeconds(dmgCooldown);
        anim.SetBool("iframe", false);
        attackable = true;
    }

    private void PhaseChange(){

        phase++;
        for(int i = 0; i < tendrils.Length; i++){
            StartCoroutine(tendrils[i].GetComponent<Tendrils>().PhaseChange(phase));
        }
        atkCooldown *= 0.5f;    
    }

    private void OnDeath(){
        Debug.Log("died");
        //play death animation
        //destroy object
        //you are win
    }

}

