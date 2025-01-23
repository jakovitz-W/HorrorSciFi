using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private Animator anim;
    private Collider2D col;
    private Rigidbody2D rb;
    [SerializeField] private float sightDistance = 3f;
    private bool hasTarget = false;
    private bool idling = true;
    [SerializeField] private GameObject sightOrigin;
    private GameObject target;

    [SerializeField] private float idleSpeed, chaseSpeed = 3f;
    private int direction = -1;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void FixedUpdate(){

        if(!hasTarget){
            RaycastHit2D ray = Physics2D.Raycast(sightOrigin.transform.position, Vector2.left, sightDistance);
            Debug.DrawRay(sightOrigin.transform.position, Vector2.left, Color.red, sightDistance);
            if(ray.collider != null){
                if(ray.collider.CompareTag("Human")){
                    //chase
                    hasTarget = true;
                    target = ray.collider.gameObject;
                    anim.SetBool("walking", true);
                    
                    target.GetComponent<HumanBehavior>().monsters.Add(this.gameObject);

                } else if(!idling){
                    //idle
                    idling = true;
                }
            }

        } else{
            //follow
            Vector2 destination = new Vector2(target.transform.position.x, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, destination, chaseSpeed * Time.deltaTime);
        }
    }

    public IEnumerator Stun(float stunTime){

        if(hasTarget){
            hasTarget = false;
            col.enabled = false;
            anim.SetBool("walking", false);
            yield return new WaitForSeconds(stunTime);

            col.enabled = true;
            hasTarget = true;
        }
    }

    public void Unlink(){
        
        target.GetComponent<HumanBehavior>().monsters.Remove(this.gameObject);
        target = null;
        hasTarget = false;
        anim.SetBool("walking", false);
    }
}
