using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrateController : MonoBehaviour
{
    
    [SerializeField] private Animator anim;
    private Collider2D col;
    void OnEnable()
    {
        col = GetComponent<Collider2D>();
    }

    public IEnumerator Melt(){
        anim.SetTrigger("Melt");
        yield return new WaitForSeconds(1f);
        col.enabled = false;
    }
}
