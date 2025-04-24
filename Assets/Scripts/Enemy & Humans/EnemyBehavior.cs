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
    public float attackBuffer = 1.5f;
    public bool isAttacking = false;
    private AudioSource footsteps = null;

    void OnEnable()
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
                    
                    if(!ray.collider.gameObject.GetComponent<HumanBehavior>().dropped){
                        currentSpeed = chaseSpeed;
                        StopCoroutine("ChooseDirection");
                        idling = false;
                        hasTarget = true;
                        target = ray.collider.gameObject;
                        anim.SetBool("walking", true);
                        
                        target.GetComponent<HumanBehavior>().Link(this.gameObject);                        
                    }


                } else if(ray.collider.CompareTag("BlocksSight")){
                    Flip();
                } 
                else if(!idling){
                    
                    idling = true;
                    StartCoroutine("ChooseDirection");
                }
            } else if(!idling){

                idling = true;
                StartCoroutine("ChooseDirection");
            }

        } else if(target != null || target.activeSelf){
            //follow
            Vector2 destination = new Vector2(target.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);

            float distance = Mathf.Abs(Vector2.Distance(this.transform.position, target.transform.position));
            if(distance <= attackBuffer){
                if(!isAttacking){
                    StartCoroutine("Attack");
                }
            }
        }

        if(idling){

            Vector2 destination = new Vector2(transform.position.x + direction, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);
        }

        if(currentSpeed != 0){
            if(footsteps == null){
                footsteps = AudioManager.Instance.PlayRepeatingAtPoint("footsteps", this.transform);                
            }
        } else{
            if(footsteps != null){
                Destroy(footsteps.gameObject);
            }
        }
    }

    private IEnumerator ChooseDirection(){

        currentSpeed = idleSpeed;
        anim.SetBool("walking", true);
        float rand = Random.Range(1f, 3f);
        yield return new WaitForSeconds(rand);
        
        currentSpeed = 0f;
        AudioManager.Instance.PlaySFXAtPoint("monster_idle", this.transform);
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
        
        AudioManager.Instance.PlaySFXAtPoint("monster_stun", this.transform);
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
        anim.SetTrigger("attack");
        isAttacking = true;

        currentSpeed = 0f;
        target.GetComponent<HumanBehavior>().Hit();
        yield return new WaitForSeconds(attackCooldown);
        currentSpeed = chaseSpeed;
        isAttacking = false;

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
