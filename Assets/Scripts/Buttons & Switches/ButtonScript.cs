using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private Affected[] affected;
    private SpriteRenderer rend;
    [SerializeField] private Sprite activeSprite, notActiveSprite;
    [SerializeField] private Light2D indicator;
    [SerializeField] private bool activated = false;    
    [SerializeField] private Animator anim;
    
    void OnEnable(){
        rend = GetComponent<SpriteRenderer>();

        if(activated){
            rend.sprite = activeSprite;
            indicator.color = Color.green;
        } else{
            rend.sprite = notActiveSprite;
            indicator.color = Color.red;
        }

        InitializeAffected();
    }

    public void ActivateAll(){

        activated = !activated;
        if(anim != null){
            anim.SetBool("pressed", true);
        }
        if(activated){
            rend.sprite = activeSprite;
            indicator.color = Color.green;
        } else{
            rend.sprite = notActiveSprite;
            indicator.color = Color.red;
        }

        for(int i = 0; i < affected.Length; i++){        
            affected[i].Activate();
        }
    }

    void InitializeAffected(){
        for(int i = 0; i < affected.Length; i++){        
            
            if(affected[i].activated){
                affected[i].Activate();
            }
        }
    }
}

[System.Serializable]
public class Affected{

    public enum ObjName{
        Default,
        ElectricDoor,
        ConveyerBelt,
        Magnet,
        Arm
    }
    [SerializeField] private ObjName name;    
    [SerializeField] private GameObject actor; //naming abstract variables is hard :(

    public bool activated = false;
    public void Activate(){
        activated = !activated;        


        switch(name){
        case ObjName.ElectricDoor:
            Door();
            break;
        case ObjName.ConveyerBelt:
            Belt();
            break;
        case ObjName.Magnet:
            Magnet();
            break;
        case ObjName.Arm:
            Arm();
            break;
        case ObjName.Default:
            Debug.Log("Default");
            break;    
        }
    }

    private void Door(){
        actor.GetComponentInChildren<Animator>().SetBool("open", activated);
        actor.GetComponent<Collider2D>().enabled = !activated;
    }

    private void Belt(){
        actor.GetComponent<SurfaceEffector2D>().enabled = activated;
        actor.GetComponent<Animator>().SetBool("Active", activated);
    }

    private void Magnet(){
        actor.GetComponent<PointEffector2D>().enabled = activated;
    }

    private void Arm(){
        activated = !activated;
        actor.GetComponent<MechanicalArm>().Activate();
    }

}
