using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private Affected[] affected;
    
    public void ActivateAll(){

        for(int i = 0; i < affected.Length; i++){        
            affected[i].Activate();
        }
    }
}

[System.Serializable]
public class Affected{

    public enum ObjName{
        Default,
        ElectricDoor,
        ConveyerBelt
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
    }

}
