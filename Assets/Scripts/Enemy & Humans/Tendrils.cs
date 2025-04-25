using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tendrils : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private AnimationClip strikeClip;
    private Vector2 targetPos;
    [SerializeField] private float speed = 4;

    private int yDir = 1;
    [SerializeField] private Transform upper, lower;
    public bool upperReached = false, lowerReached = true;

    public bool striking = false;
    private float originX;
    private Vector2 startPos;
    float originSpeed;

    public bool isVertical = false;
    public bool onWall = false;
    public bool stationary = false;
    private float atckCooldown = 5f;
    

    void OnEnable(){
        anim = GetComponent<Animator>();
        startPos = transform.position;
        originX = transform.position.x;
        originSpeed = speed;
    }

    public void Reset(){
        anim.ResetTrigger("strike");
        StopAllCoroutines();
        transform.position = startPos;
    }


    void FixedUpdate(){
        
        if(!stationary){

            if(!striking){

                targetPos = new Vector2(originX, transform.position.y + yDir);

                if(transform.position.y >= upper.position.y && !upperReached){
                    upperReached = true;
                    lowerReached = false;
                    yDir = -yDir;
                    //Debug.Log(yDir);
                }
                
                if(transform.position.y <= lower.position.y && !lowerReached){
                    upperReached = false;
                    lowerReached = true;
                    yDir = -yDir;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }

    }

    public IEnumerator Strike(){

        float rand;
        bool atck_decision = true;

        if(isVertical || onWall){
            rand = Random.Range(0, 10);
            if(rand < 5){
                atck_decision = false;
            }
        }

        if(atck_decision){
            striking = true;
            speed -= speed * 0.5f;
            rand = Random.Range(0.7f, 1f);
            yield return new WaitForSeconds(rand);

            AudioManager.Instance.PlaySFXAtPoint("boss_attack", this.transform);
            anim.SetTrigger("strike");
            yield return new WaitForSeconds(strikeClip.length);

            speed = originSpeed;                    
        }
        striking = false;

        yield return new WaitForSeconds(atckCooldown);
        if(this.gameObject.activeSelf){
            StartCoroutine("Strike");   
        }
        
    }    
}
