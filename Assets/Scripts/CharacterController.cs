using UnityEngine;
using UnityEngine.Events;


//modified from brackeys 2D movement tutorial
public class CharacterController : MonoBehaviour
{
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    private Rigidbody2D rb;
    private bool m_FacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;
    [SerializeField] private int tiltAmount = 10;
    [SerializeField] private float tiltSpeed = 50f;
    private Quaternion targetRotation;

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float moveX, float moveY){
    

        Vector3 targetVelocity = new Vector2(moveX * 10f, moveY * 10f);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if(moveX > 0 && !m_FacingRight){
            Flip();
        }
        else if(moveX < 0 && m_FacingRight){
            Flip();
        }

        if(moveX > 0){
            targetRotation = Quaternion.Euler(0f, 0f, -tiltAmount);
        } else if(moveX < 0){
            targetRotation = Quaternion.Euler(0f, 0f, tiltAmount);
        } else{
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime);

    }
    private void Flip(){
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *=-1;
        transform.localScale = theScale;
    }
}

