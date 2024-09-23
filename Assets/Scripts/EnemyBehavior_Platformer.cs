using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Platformer : MonoBehaviour
{
    public enum EnemyType{
        Debug, //stationary
        Flying, //up + down 
        Wandering, //walk back and forth between walls
        Tracking //walk towards x direction of player
    }
    public EnemyType type;
    private Rigidbody2D rb;
    private GameObject player;
    public float upperLimit, lowerLimit = 0f;
    public LayerMask mask;
    public int speed;
    private bool facingRight = true;
    public bool wallHit = false;
    public Transform wallCheck;
    public float checkRange = 2.0f;
    private bool canTrack = false;

    //we want the flying type to move upwards first
    private bool upperReached = false;
    private bool lowerReached = true; 
    private float yTarget;
    public float flySpeed = 3f;
    private float y_initial;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player"); 
        y_initial = transform.position.y;
    }

    void FixedUpdate()
    {
        switch((int)type){
            case 0: //debug
                //Debug.Log("Debug type");
                break;
            case 1: //flying
                Fly(upperLimit, lowerLimit);
                break;
            case 2: //wandering
                Wander();
                break;
            case 3: //tracking
                if(canTrack){
                    Track();
                }
                break;
            default: //not neccessary but good practice to include a default case 
                break;
        }
    }

    public void Fly(float upper, float lower){
        
        if(!upperReached && lowerReached){
            yTarget = upper + y_initial;
        } else if(upperReached && !lowerReached){
            yTarget = y_initial - lower;
        }

        if(transform.position.y > yTarget - .05f && transform.position.y < yTarget + .05f && lowerReached){
            //Debug.Log("Upper reached");
            upperReached = true;
            lowerReached = false;
        } else if(transform.position.y > yTarget - .05f && transform.position.y < yTarget + .05f && upperReached){
            //Debug.Log("Lower reached");
            lowerReached = true;
            upperReached = false;
            
        }
        //Debug.Log(yTarget);
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, yTarget), flySpeed * Time.deltaTime);
    }

    public void Wander(){
        
        if(Physics2D.OverlapCircle(wallCheck.position, checkRange, mask) && !wallHit){
            wallHit = true;
            Flip();
            speed = 0 - speed;
        }
        rb.velocity = new Vector3(speed * Time.deltaTime * 25, 0, 0);
    }

    public void Track(){
        Vector2 playerPos = player.transform.position;
        float xDist = this.transform.position.x -playerPos.x;

        if(xDist > 0 && facingRight){ //player is to the left
            Flip();
        } else if(xDist < 0 && !facingRight){ //player is to the right
            Flip();
        }

        transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerPos.x, this.transform.position.y), speed * Time.deltaTime);
    }

    private void Flip(){
        facingRight = !facingRight;
        wallHit = false;

        Vector3 theScale = transform.localScale;
        theScale.x *=-1;
        transform.localScale = theScale;
    }

    void OnTriggerEnter2D(Collider2D other){

        if(other.gameObject.CompareTag("GroundCheck")){
            Destroy(gameObject);
            player.GetComponent<PlayerMovement>().isInvincible = true;
        } else if(other.gameObject.CompareTag("Activator")){
            canTrack = true;
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.CompareTag("Player")){
            player.GetComponent<PlayerMovement>().OnDeath(false);
        } else if(col.gameObject.CompareTag("Enemy")){
            wallHit = true;
            Flip();
            speed = 0 - speed;
        }
    }

    void OnDrawGizmosSelected(){
        if(wallCheck == null)
            return;

        Gizmos.DrawWireSphere(wallCheck.position, checkRange);
    }
}
