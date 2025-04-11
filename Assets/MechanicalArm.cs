using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalArm : MonoBehaviour
{
    public GameObject target, targetParent;
    private Vector2 origin, targetOrigin;
    private Vector2 targetPos;
    public bool move;
    public int startDir = -1;
    public float moveSpeed = 3;
    public int currentDir;

    void OnEnable(){
        origin = transform.position;
        targetOrigin = targetParent.transform.position;
        currentDir = startDir;
    }
    public void Activate(){
        move = true;
    }

    void Update(){
        if(move){
            float step = moveSpeed * Time.deltaTime;

            targetPos = new Vector2(transform.position.x + currentDir, transform.position.y);
            
            if(currentDir == -startDir){ //moving back to origin
                if(transform.position.x >= origin.x){    
                    currentDir = startDir;
                    move = false;
                }
                targetPos = origin;
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);

            if(target.transform.position.x <= targetOrigin.x){

                if(target.transform.parent == this.transform){
                    target.transform.parent = targetParent.transform;
                    currentDir = -currentDir;                    
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col){

        if(col.gameObject == target){

            if(target.transform.parent != this.transform){
                target.transform.parent = this.transform;
                currentDir = -currentDir;
            }
        } else if(col.gameObject.tag != "Player"){
            currentDir = -currentDir;
        }
    }
}
