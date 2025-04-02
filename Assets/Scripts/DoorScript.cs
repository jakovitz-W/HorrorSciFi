using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DoorScript : MonoBehaviour
{
    private SpriteRenderer rend;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Light2D indicator;
    public bool requireKey = true; //for re-use of the prefab in hallways, true by default
    public bool isBoss;

    void Awake(){
        rend = GetComponent<SpriteRenderer>();

        if(!requireKey){
            SetUnlocked();
        }
    }
    public void SetUnlocked(){
        rend.sprite = unlockedSprite;
        indicator.color = Color.green;
    }
}
