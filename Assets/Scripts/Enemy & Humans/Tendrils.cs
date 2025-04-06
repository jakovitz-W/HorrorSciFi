using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tendrils : MonoBehaviour
{
    private Transform player;
    private float xBuff = 3;
    private Vector2 targetPos;
    [SerializeField] private float speed = 4;
    [SerializeField] private int xDir;

    private int yDir = 1;
    [SerializeField] private Transform upper, lower;
    private bool upperReached = false, lowerReached = true;

    public bool striking = false;
    private float originX;
    float originSpeed;
    
    /*Add some x variability for normal movement*/
    void OnEnable(){
        player = GameObject.FindWithTag("Player").transform;
        originX = transform.position.x;
        originSpeed = speed;
    }
    public IEnumerator PhaseChange(){

        originSpeed = speed;
        speed = 0;
        yield return new WaitForSeconds(2f);
        float mod = Random.Range(1f,2f);
        speed = originSpeed * mod;
        xBuff -= 0.25f;
    }


    void FixedUpdate(){
        
        if(!striking){

            targetPos = new Vector2(originX, yDir *(transform.position.y + 1));

            if(transform.position.y >= upper.position.y && !upperReached){
                upperReached = true;
                lowerReached = false;
                yDir = -yDir;
            } else if(transform.position.y <= lower.position.y && !lowerReached){
                upperReached = false;
                lowerReached = true;
                yDir = -yDir;
            }
        } else{
            if(transform.position.x == originX){ 
                striking = false;
            } else{
                striking = true;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    public IEnumerator Strike(){

        float x;

        speed -= speed * 0.5f;
        yield return new WaitForSeconds(1f);

        //calculate target position
        if(player.position.x < transform.position.x){ //left side

            if(xDir == -1){ //towards player
                x = Random.Range(player.position.x, (transform.position.x - xBuff)); //pick point between player + buffer & position - buffer
            } else{//pick random
                x = Random.Range(transform.position.x - xBuff, transform.position.x);
            }
        } else{ //right side

            if(xDir == 1){//towards player
                x = Random.Range(player.position.x, (transform.position.x + xBuff)); //pick point between player - buffer & position + buffer
            }else{ //pick random
                x = Random.Range(transform.position.x, transform.position.x + xBuff);
            }
        }
        
        striking = true;
        speed = originSpeed * 4f;
        targetPos = new Vector2(x, transform.position.y);
        yield return new WaitForSeconds(1f);

        speed *= .7f;
        targetPos = new Vector2(originX, transform.position.y);
        yield return new WaitForSeconds(0.5f);
        
        speed = originSpeed;
    }    
}
