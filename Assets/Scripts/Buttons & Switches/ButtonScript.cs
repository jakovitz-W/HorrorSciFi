using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private Affected[] affected;
    private SpriteRenderer rend;
    [SerializeField] private Sprite activeSprite, notActiveSprite;
    [SerializeField] private Light2D[] indicators;
    [SerializeField] private bool activated = false;    
    [SerializeField] private Animator anim;
    public bool isTiny;
    public float default_intensity; //for tiny button indicators only
    public bool pressed = false; //for and gate style buttons

    void Awake(){
        rend = GetComponent<SpriteRenderer>();

        if(activated){
            rend.sprite = activeSprite;

            if(isTiny){
                for(int i = 0; i < indicators.Length; i++){
                    
                    indicators[i].color = Color.red;
                    indicators[i].intensity = default_intensity; 
                }
            } else{
                for(int i = 0; i < indicators.Length; i++){
                    indicators[i].color = Color.green;
                }
            }

        } else{
            rend.sprite = notActiveSprite;

            if(isTiny){
                for(int i = 0; i < indicators.Length; i++){
                    indicators[i].intensity = 0f;
                }
            }
            for(int i = 0; i < indicators.Length; i++){
                indicators[i].color = Color.red;
            }
        }

        InitializeAffected();
    }

    public void ActivateAll(){

        pressed = true;
        activated = !activated;
        if(anim != null){
            anim.SetBool("pressed", true);
        }
        if(activated){
            rend.sprite = activeSprite;

            if(isTiny){
                AudioManager.Instance.PlaySFXAtPoint("tiny_button", this.transform);
                for(int i = 0; i < indicators.Length; i++){
                    indicators[i].intensity = default_intensity;
                }
            }else{
                for(int i = 0; i < indicators.Length; i++){
                    indicators[i].color = Color.green;
                }                
            }

        } else{
            AudioManager.Instance.PlaySFXAtPoint("tiny_button", this.transform);            
            rend.sprite = notActiveSprite;
            for(int i = 0; i < indicators.Length; i++){
                indicators[i].color = Color.red;
            }
            
            if(isTiny){
                for(int i = 0; i < indicators.Length; i++){
                    indicators[i].intensity = 0f;
                }
            }
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
        Arm,
        Trapdoor
    }
    [SerializeField] private ObjName name;    
    [SerializeField] private GameObject actor; //naming abstract variables is hard :(
    private AudioSource audioSource;

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
        case ObjName.Trapdoor:
            Trapdoor();
            break;
        case ObjName.Default:
            Debug.Log("Default");
            break;    
        }
    }

    private void Door(){
        actor.GetComponentInChildren<Animator>().SetBool("open", activated);
        actor.GetComponent<Collider2D>().enabled = !activated;
        AudioManager.Instance.PlaySFXAtPoint("electric_door", actor.transform);
    }

    private void Belt(){
        actor.GetComponent<SurfaceEffector2D>().enabled = activated;
        actor.GetComponent<Animator>().SetBool("Active", activated);
        if(activated){
            audioSource = AudioManager.Instance.PlayRepeatingAtPoint("mechanical_arm", actor.transform);            
        } else{
            UnityEngine.Object.Destroy(audioSource.gameObject); //unity doesn't like destroying objects outside of monobehavior
        }

    }

    private void Magnet(){
        actor.GetComponent<PointEffector2D>().forceMagnitude = -500;
        if(activated){
            audioSource = AudioManager.Instance.PlayRepeatingAtPoint("magnet", actor.transform);            
        } else{
            UnityEngine.Object.Destroy(audioSource.gameObject);
        }
    }

    private void Arm(){
        activated = !activated;
        actor.GetComponent<MechanicalArm>().Activate();
    }

    private void Trapdoor(){
        actor.GetComponent<trapdoor>().SetUnlockState();
    }

}
