using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject[] tendrils;
    [SerializeField] private float atkCooldown = 3f;
    [SerializeField] private float speedIncr;
    [SerializeField] private float dmgCooldown;
    private bool isAttacking = false;
    private bool attackable = true;
    private int phase = 1;
    private int hits = 0;

    public bool testStrike = false;
    
    void OnEnable(){
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
    }

    private IEnumerator AttackSequence(){

        float cd = Random.Range(0.5f, atkCooldown);
        for(int i = 0; i < tendrils.Length; i++){
            StartCoroutine(tendrils[i].GetComponent<Tendrils>().Strike());
        }
        yield return new WaitForSeconds(cd);

        if(attackable){
            StartCoroutine("AttackSequence");
        }
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
        StopCoroutine("AttackSequence");
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
        StartCoroutine("AttackSequence");
    }

    private void OnDeath(){
        //play death animation
        //destroy object
        //you are win
    }

}
