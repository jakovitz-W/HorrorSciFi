using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private Animator anim;
    private Collider2D col;
    private Rigidbody2D rb;
    [SerializeField] private float sightDistance = 3f;
    public bool hasTarget = false;
    private bool idling = true;
    [SerializeField] private GameObject sightOrigin;
    private GameObject target;

    [SerializeField] private float idleSpeed, chaseSpeed = 3f;
    public float currentSpeed = 1f;
    public int direction = -1; //change in inspector for monsters placed facing right
   
    public float attackCooldown = 1f;
    public float attackBuffer = 2f;
    public bool isAttacking = false;


    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        StartCoroutine("ChooseDirection");
    }

    void FixedUpdate(){

        if(!hasTarget){

            RaycastHit2D ray = Physics2D.Raycast(sightOrigin.transform.position, -direction * Vector2.left,sightDistance);
            //Debug.DrawRay(sightOrigin.transform.position, -direction * Vector2.left, Color.red, sightDistance);
            
            if(ray.collider != null){
                if(ray.collider.CompareTag("Human")){

                    currentSpeed = chaseSpeed;
                    StopCoroutine("ChooseDirection");
                    idling = false;
                    hasTarget = true;
                    target = ray.collider.gameObject;
                    anim.SetBool("walking", true);
                    
                    target.GetComponent<HumanBehavior>().monsters.Add(this.gameObject);

                } else if(!idling){
                    //idle
                    idling = true;

                    StartCoroutine("ChooseDirection");
                }
            } else if(!idling){

                idling = true;
                StartCoroutine("ChooseDirection");
            }

        } else if(target != null){
            //follow
            Vector2 destination = new Vector2(target.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);

            if(transform.position.x - attackBuffer <= target.transform.position.x){

                if(!isAttacking){
                    StartCoroutine("Attack");
                }
            }
        }

        if(idling){

            Vector2 destination = new Vector2(transform.position.x + direction, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);
        }
    }

    private IEnumerator ChooseDirection(){


        currentSpeed = idleSpeed;
        anim.SetBool("walking", true);
        float rand = Random.Range(1f, 3f);
        yield return new WaitForSeconds(rand);
        
        currentSpeed = 0f;

        Flip();

        anim.SetBool("walking", false);
        rand = Random.Range(2f, 4f);
        yield return new WaitForSeconds(rand);

        currentSpeed = idleSpeed;
        anim.SetBool("walking", true);

        if(!hasTarget){
            StartCoroutine("ChooseDirection");
        } else{
            StopCoroutine("ChooseDirection");
        }
    }

    public IEnumerator Stun(float stunTime){
        
        col.enabled = false;
        currentSpeed = 0f;
        
        if(hasTarget){
            StopCoroutine("Attack");
        } else{
            StopCoroutine("ChooseDirection");
        }

        anim.SetBool("walking", false);
        yield return new WaitForSeconds(stunTime);
        anim.SetBool("walking", true);

        if(idling){
            currentSpeed = idleSpeed;
            StartCoroutine("ChooseDirection");
        } else{
            currentSpeed = chaseSpeed;
        }

        col.enabled = true;
    }

    private IEnumerator Attack(){
        //play attack animation
        //Debug.Log("Attack");
        anim.SetBool("walking", false);
        isAttacking = true;
        currentSpeed = 0f;

        target.GetComponent<HumanBehavior>().Hit();
        yield return new WaitForSeconds(attackCooldown);
        
        isAttacking = false;
        currentSpeed = chaseSpeed;
        anim.SetBool("walking", true);
    }

    public void Flip(){
        direction = -direction;

        Vector3 theScale = transform.localScale;
        theScale.x *=-1;
        transform.localScale = theScale;
    }

    public void Unlink(){
        
        target = null;
        hasTarget = false;
        StopCoroutine("Attack");
        anim.SetBool("walking", false);
    }
}
