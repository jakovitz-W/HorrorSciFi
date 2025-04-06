using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject[] tendrils;
    [SerializeField] private float atkCooldown = 5f;
    [SerializeField] private float speedIncr;
    [SerializeField] private float dmgCooldown;
    private bool isAttacking = false;
    private bool attackable = true;
    private int phase = 1;
    private int hits = 0;

    public bool testStrike = false;
    public bool startBoss = false;
    public bool bossActive = false;

    public IEnumerator StartFight(){
        //start first section of boss theme
        //intro dialogue
        yield return new WaitForSeconds(1f);
        bossActive = true;
        StartCoroutine("AttackSequence");
    }

    void FixedUpdate(){

    
        /*delete later*/
        if(testStrike){
            testStrike = false;
            for(int i = 0; i < tendrils.Length; i++){
                StartCoroutine(tendrils[i].GetComponent<Tendrils>().Strike());
            }
        }
        
        for(int j = 0; j < tendrils.Length; j++){
            if(tendrils[j].GetComponent<Tendrils>().striking){
                isAttacking = true;
                break;
            } else{
                isAttacking = false;
            }
        }

        if(!isAttacking && bossActive){
            StartCoroutine("AttackSequence");
        }
    
    }

    private IEnumerator AttackSequence(){

        float cd = Random.Range(5f, atkCooldown);
        for(int i = 0; i < tendrils.Length; i++){
            StartCoroutine(tendrils[i].GetComponent<Tendrils>().Strike());
        }
        yield return new WaitForSeconds(cd);
        isAttacking = false;
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
        PhaseChange();
        //flashing opacity animation
        yield return new WaitForSeconds(dmgCooldown);
        //return to full opacity
        attackable = true;
    }

    private void PhaseChange(){

        phase++;
        for(int i = 0; i < tendrils.Length; i++){
            StartCoroutine(tendrils[i].GetComponent<Tendrils>().PhaseChange());
        }
        atkCooldown *= 0.5f;    
    }

    private void OnDeath(){
        //play death animation
        //destroy object
        //you are win
    }

}

