using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehavior : MonoBehaviour
{
    /*Script variables*/
    private Animator anim;
    public string type;
    private Rigidbody2D rb;
    public string dialogueKey;  
    public float health =100, maxHealth = 100;
    public FloatingHealthBar healthBar;
    [SerializeField] private float damageAmnt = 10;

    [SerializeField] private float idleSpeed, fleeSpeed, followSpeed;
    public int direction = 1;
    private float currentSpeed;
    public bool isFollowing = false;
    public bool isFrightened = false;
    private bool stuck = false;

    public List<GameObject> monsters;
    private GameObject runFrom;
    [SerializeField] private float xBuffer = 2f;
    private float yBuffer = 2f;
    private Vector2 destination;

    private Transform player;
    private bool idling = true;
    [SerializeField] private bool isClimbing = false;
    private LadderPathing ladderPathing;
    [SerializeField] private float sightDistance = 4f;
    [SerializeField] private Collider2D mainCol;
    public bool dropped = false;
    private bool stopped = false;
    private Transform dropoff;
    private bool alive;

    private AudioSource footsteps = null;

    void OnEnable(){
        monsters = new List<GameObject>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        ladderPathing = GetComponent<LadderPathing>();
        ladderPathing.enabled = false;
        dropped = false;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine("ChooseDirection");
    }

    void FixedUpdate(){

        if(dropped){
            if(Mathf.Abs(transform.position.x - dropoff.position.x) > 0){
                
                if(direction == 1 && dropoff.position.x < transform.position.x){
                    Flip();
                }else if(direction == -1 && dropoff.position.x > transform.position.x){
                    Flip();
                }

                destination = new Vector2(dropoff.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);
            } else{
                currentSpeed = 0;
                anim.SetBool("isWalking", false);
            }
        }

        if(!isFollowing && !isFrightened && !dropped){

            if(!idling){
                idling = true;
                StartCoroutine("ChooseDirection");
            } else{
                destination = new Vector2(transform.position.x + direction, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);
            }
        }


        if(isFrightened && !dropped && !isFollowing){

            CheckStuck();

            if(!stuck && !stopped){
                StopCoroutine("ChooseDirection");
                anim.SetBool("isWalking", true);

                if(monsters.Count != 0){
                    runFrom = ReturnClosest(); //watch out for async issues
                    
                    if(direction != runFrom.GetComponent<EnemyBehavior>().direction){
                        Flip();
                    }

                }

                destination = new Vector2(transform.position.x + direction, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);   
            }
        }

        if(isFollowing && !isClimbing &&!dropped){

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, sightDistance, LayerMask.GetMask("GameplayObjects"));

            foreach(Collider2D hit in hits){

                if(hit.gameObject.tag == "Ladder" && !isClimbing){
                    if(Mathf.Abs(player.transform.position.y - transform.position.y) >= yBuffer){
                        isClimbing = true;
                        ladderPathing.enabled = true;
                        ladderPathing.GrabNodes(hit.gameObject);
                    }
                }
            }

            if(Mathf.Abs(player.transform.position.x - transform.position.x) <= xBuffer){
                currentSpeed = 0;
                anim.SetBool("isWalking", false);

            } else{
                currentSpeed = followSpeed;
                anim.SetBool("isWalking", true);

                if(player.position.x > transform.position.x && direction == -1){
                    Flip();
                } else if(player.position.x < transform.position.x && direction == 1){
                    Flip();
                }                
            }

            destination = new Vector2(player.position.x + direction, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);
        }

        if(isClimbing){

            anim.SetBool("isWalking", true);

            if(direction == 1 && ladderPathing.targetLadder.position.x < transform.position.x){
                Flip();
            }else if(direction == -1 && ladderPathing.targetLadder.position.x > transform.position.x){
                Flip();
            }

            if(ladderPathing.endPoint.position.x > transform.position.x && direction == -1 && ladderPathing.onLadder){
                Flip();
            } else if(ladderPathing.endPoint.position.x < transform.position.x && direction == 1 && ladderPathing.onLadder){
                Flip();
            }            

            if(ladderPathing.endReached){
                anim.SetBool("isWalking", false);
                ladderPathing.enabled = false;
                isClimbing = false;
            }
        }

        if(currentSpeed != 0 && !isClimbing){
            if(footsteps == null){
                footsteps = AudioManager.Instance.PlayRepeatingAtPoint("footsteps", this.transform);                
            }
        } else{
            if(footsteps != null){
                Destroy(footsteps.gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col){

        if(col.gameObject.tag == "Dropoff"){
            dropoff = col.gameObject.transform;
            DropOff();
        } else if(col.gameObject.tag == "Ladder"){
            rb.gravityScale = 0;
        }
    }

    void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.tag == "Ladder"){
            rb.gravityScale = 1;
        }
    }

    private void CheckStuck(){
        
        if(isFollowing){
            return;
        }
        
        RaycastHit2D forwardRay = Physics2D.Raycast(transform.position, -direction * Vector2.left, sightDistance);

        bool left = false;
        bool right = false;
        
        for(int i = 0; i < monsters.Count; i++){

            float distance = Mathf.Abs(transform.position.x - monsters[i].transform.position.x);

            if(monsters[i].transform.position.x < transform.position.x){
                if(distance <= 4.5){
                    left = true;
                }
            }
            if(monsters[i].transform.position.x > transform.position.x){
                if(distance <= 4.5){
                    right = true;
                }
            }
        }

        if(forwardRay.collider!= null && forwardRay.collider.CompareTag("BlocksSight")){
            if(direction == 1){
                right = true;
            }

            if(direction == -1){
                left = true;
            }
        }

        if(left && right){
            stuck = true;
            anim.SetBool("isWalking", false);
        } else{
            stuck = false;
        }
    }
    private GameObject ReturnClosest(){

        GameObject closest = monsters[0];
        float distance = Mathf.Abs(transform.position.x - monsters[0].transform.position.x);
        float last = distance;

        for(int i = 0; i < monsters.Count; i++){
            
            distance = Mathf.Abs(transform.position.x - monsters[i].transform.position.x);
            if(distance <= last){
                closest = monsters[i];
            }
        }

        return closest;
    }

    private IEnumerator ChooseDirection(){

        currentSpeed = idleSpeed;
        anim.SetBool("isWalking", true);

        float rand = Random.Range(1f, 3f);
        yield return new WaitForSeconds(rand);
        
        currentSpeed = 0f;

        Flip();

        anim.SetBool("isWalking", false);
        rand = Random.Range(2f, 4f);
        yield return new WaitForSeconds(rand);

        currentSpeed = idleSpeed;
        anim.SetBool("isWalking", true);
        
        if(!isFrightened && !isFollowing){
            StartCoroutine("ChooseDirection");
        } else{
            StopCoroutine("ChooseDirection");
        }
    }

    public void Hit(){

        if(health > 0){
            StartCoroutine("TakeDamage");
        } else{
            AudioManager.Instance.PlaySFXAtPoint("human_die", monsters[0].transform);
            OnDeath();
        }
    }
    private IEnumerator TakeDamage(){
        
        stopped = true;
        anim.SetBool("isWalking", false);
        currentSpeed = 0f;
        health -= damageAmnt;
        healthBar.UpdateHealthBar(health, maxHealth);

        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(.05f);
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(2f);
        currentSpeed = fleeSpeed;
        anim.SetBool("isWalking", true);
        stopped = false;
    }

    public void Link(GameObject monster){

        monsters.Add(monster);

        if(!isFollowing){
            currentSpeed = fleeSpeed;
            isFrightened = true;
        }
    }

    public void Unlink(){ //spaghetti-esque, don't do this
        for(int i = 0; i < monsters.Count; i++){
            monsters[i].GetComponent<EnemyBehavior>().Unlink();
        }
    }

    public void DropOff(){
        if(!isFollowing){
            return;
        }
        
        dropped = true;

        player.gameObject.GetComponent<PlayerInteractions>().RemoveHuman(this.gameObject);
        mainCol.enabled = false;
        isFollowing = false;
        isClimbing = false;
        isFrightened = false;
        currentSpeed = followSpeed;
        Unlink();
        StartCoroutine("ChooseDirection");
    }

    void OnDeath(){
        //remove references to human in all following monsters
        for(int i = 0; i < monsters.Count; i++){
            monsters[i].GetComponent<EnemyBehavior>().Unlink();
        }
        alive = false;
        gameObject.SetActive(false);
    }

    public void Flip(){
        direction = -direction;

        Vector3 theScale = transform.localScale;
        theScale.x *=-1;
        transform.localScale = theScale;
    }
}
