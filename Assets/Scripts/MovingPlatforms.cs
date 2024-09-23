using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    public enum PlatformType{
        horizontal,
        vertical
    }
    public PlatformType type;
    public float speed = 3f;
    public float upper;
    public float lower;
    private bool upperReached = false;
    private bool lowerReached = true;
    private float target;
    private float y_initial;
    private float x_initial;
    private bool playerOn;

    void Awake(){
        x_initial = transform.position.x;
        y_initial = transform.position.y;
    }
    void Update()
    {
        //not using switch/case here because it's only two types

        if((int)type == 0){//horizontal
            if(!upperReached && lowerReached){
                target = upper + x_initial;
            } else if(upperReached && !lowerReached){
                target = x_initial - lower;
            }

            if(transform.position.x > target - .05f && transform.position.x < target + .05f && lowerReached){
                //Debug.Log("Upper reached");
                upperReached = true;
                lowerReached = false;
            } else if(transform.position.x > target - .05f && transform.position.x < target + .05f && upperReached){
                //Debug.Log("Lower reached");
                lowerReached = true;
                upperReached = false;
            
            }
           
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(target, transform.position.y), speed * Time.deltaTime);

        } else if((int)type == 1){//vertical
            if(!upperReached && lowerReached){
                target = upper + y_initial;
            } else if(upperReached && !lowerReached){
                target = y_initial - lower;
            }

            if(transform.position.y > target - .05f && transform.position.y < target + .05f && lowerReached){
                //Debug.Log("Upper reached");
                upperReached = true;
                lowerReached = false;
            } else if(transform.position.y > target - .05f && transform.position.y < target + .05f && upperReached){
                //Debug.Log("Lower reached");
                lowerReached = true;
                upperReached = false;
            
            }
        
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, target), speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.CompareTag("Player") && !playerOn){
            col.gameObject.transform.SetParent(transform);
            playerOn = true;
        }
    }

    void OnCollisionExit2D(Collision2D col){
                
        if(col.gameObject.CompareTag("Player") && playerOn){
            col.gameObject.transform.SetParent(null);
            playerOn = false;
        }
    }
}
