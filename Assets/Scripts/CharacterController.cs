using UnityEngine;
using UnityEngine.Events;


//modified from brackeys 2D movement tutorial
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;
    [Range(1, 2)][SerializeField] private float m_SprintSpeed = 1.5f;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Transform m_CeilingCheck;
    [SerializeField] private Collider2D m_CrouchDisableCollider;

    const float k_GroundedRadius = .02f;
    public bool m_Grounded;
    const float k_CielingRadius = .05f;
    private Rigidbody2D rb;
    private bool m_FacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>{}
    public BoolEvent onCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();

        if(OnLandEvent == null)
            OnLandEvent = new UnityEvent();
        if(onCrouchEvent == null)
            onCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate(){
        bool wasGrounded = m_Grounded;
        if(rb.velocity.y == 0){
            m_Grounded = true;
        }
        else{
            m_Grounded = false;
        }
    }

    public void Move(float move, bool crouch, bool jump, bool sprint, bool ignoreGround){
        
		if(!crouch){
            if(Physics2D.OverlapCircle(m_CeilingCheck.position, k_CielingRadius, m_WhatIsGround)){
                crouch = true;
            }
        }

        if(m_Grounded || m_AirControl){
            if(crouch){
                if(!m_wasCrouching){
                    m_wasCrouching = true;
                    onCrouchEvent.Invoke(true);
                }

                move *= m_CrouchSpeed;

                if(m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else{
                if(m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;
                if(m_wasCrouching){
                    m_wasCrouching = false;
                    onCrouchEvent.Invoke(false);
                }
                if(sprint){
                    move*= m_SprintSpeed;
                }
            }

            Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            if(move > 0 && !m_FacingRight){
                Flip();
            }
            else if(move < 0 && m_FacingRight){
                Flip();
            }
        }

        if(jump){
            if(m_Grounded || ignoreGround){
                rb.AddForce(new Vector2(0f, m_JumpForce));
            }
        }

    }
    private void Flip(){
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *=-1;
        transform.localScale = theScale;
    }

    public void ChangeGravity(float modifier){ //for conditions like upsideDown sections + low grav areas
        rb.gravityScale += modifier;
    }
}

