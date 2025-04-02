using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] protected Transform trackingTarget;
    [SerializeField] protected float followSpeed;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    public float zPos = 0;
    [SerializeField] protected bool isXLocked = false;
    [SerializeField] protected bool isYLocked = false;
    [SerializeField] protected bool canFollow = true;
    [SerializeField] protected bool autoScroll = false;


    protected void Update()
    {
        //checking both parameters so it doesn't end up in a hostage situation
        if(canFollow && !autoScroll){
            float xTarget = trackingTarget.position.x + xOffset;
            float yTarget = trackingTarget.position.y + yOffset;
            float xNew = transform.position.x;
            float yNew = transform.position.y;

            if(!isXLocked){
                xNew = Mathf.Lerp(transform.position.x, xTarget, Time.deltaTime * followSpeed);
            }
            if(!isYLocked){
                yNew = Mathf.Lerp(transform.position.y, yTarget, Time.deltaTime * followSpeed);
            }
            transform.position = new Vector3(xNew, yNew, zPos);
        } 
        else if(autoScroll && !canFollow){

            if(!isXLocked && isYLocked){ //move right
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 1, transform.position.y, -1), followSpeed * Time.deltaTime);
            } else if(isXLocked && !isYLocked){ //move up
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y + 1, -1), followSpeed * Time.deltaTime);
            }
        }
    }

    //using tags instead of gameobject names for reusability
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "YUnlock"){
            isYLocked = false;
        }
        else if(other.gameObject.tag == "YLock"){
            isYLocked = true;
        }
        else if(other.gameObject.tag == "XUnlock"){
            isXLocked = false;
        }
        else if(other.gameObject.tag == "XLock"){
            isXLocked = true;
        } else if(other.gameObject.tag == "AutoScrollEnable"){
            canFollow = false;
            autoScroll = true;
        } else if(other.gameObject.tag == "AutoScrollDisable"){
            canFollow = true;
            autoScroll = false;
        }

    }  
}
