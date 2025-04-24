using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tendrils : MonoBehaviour
{
    private Animator anim;
    private Transform player;
    private float xBuff = 3;
    private Vector2 targetPos;
    [SerializeField] private float speed = 4;
    [SerializeField] private int xDir;

    private int yDir = 1;
    [SerializeField] private Transform upper, lower;
    public bool upperReached = false, lowerReached = true;

    public bool striking = false;
    private float originX;
    private Vector2 startPos;
    float originSpeed;

    public bool shouldMove = false;
    public bool isVertical = false;
    public bool onWall = false;
    public bool stationary = false;
    public bool active = false;
    

    void OnEnable(){
        anim = GetComponent<Animator>();
        startPos = transform.position;
        player = GameObject.FindWithTag("Player").transform;
        originX = transform.position.x;
        originSpeed = speed;
    }

    public void Reset(){
        transform.position = startPos;
        shouldMove = false;
    }

    public IEnumerator PhaseChange(int phase){

        originSpeed = speed;
        speed = 0;
        yield return new WaitForSeconds(2f);
        float mod = Random.Range(1f,2f);
        speed = originSpeed * mod;
        xBuff -= 0.25f;

        if(phase == 2){
            if(isVertical){
                anim.SetTrigger("emerge_vertical");
                active = true;
            }
        }

        if(phase == 3){
            if(onWall){
                anim.SetTrigger("emerge_horizontal");
                active = true;
                shouldMove = true;
            }
        }
    }


    void FixedUpdate(){
        
        if(shouldMove && !stationary){
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

        if(!active){
            yield break;
        }

        float rand;

        if(isVertical || onWall){
            rand = Random.Range(0, 10);
            if(rand < 7){
                yield break;
            }
        }

        speed -= speed * 0.5f;
        rand = Random.Range(0.7f, 1f);
        yield return new WaitForSeconds(rand);

        AudioManager.Instance.PlaySFXAtPoint("boss_attack", this.transform);
        anim.SetBool("strike", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("strike", false);

        speed = originSpeed;
    }    
}
