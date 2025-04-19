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
    public bool upperReached = false, lowerReached = true;

    public bool striking = false;
    private float originX;
    float originSpeed;

    public bool shouldMove = false;
    
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
        
        if(shouldMove){
            if(!striking){

                targetPos = new Vector2(originX, transform.position.y + yDir);

                if(transform.position.y >= upper.position.y && !upperReached){
                    upperReached = true;
                    lowerReached = false;
                    yDir = -yDir;
                    Debug.Log(yDir);
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

        float x;

        speed -= speed * 0.5f;
        float rand = Random.Range(0.7f, 1f);
        yield return new WaitForSeconds(rand);

        GetComponent<Animator>().SetBool("strike", true);
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetBool("strike", false);

        speed = originSpeed;
    }    
}
