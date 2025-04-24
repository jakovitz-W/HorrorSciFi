using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapdoor : MonoBehaviour
{
    public ButtonScript button1, button2;
    [SerializeField] private Collider2D platform;
    [SerializeField] private SpriteRenderer sprite;

    public void SetUnlockState(){

        if(button1.pressed && button2.pressed){
            platform.enabled = false;
            sprite.color = new Color(1f, 1f, 1f, .5f);
        }
    }
}
