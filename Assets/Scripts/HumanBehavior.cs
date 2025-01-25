using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehavior : MonoBehaviour
{
    public string type;
    public GameObject dropoff;
    public string dialogueKey;  
    public float health, maxHealth;
    public FloatingHealthBar healthBar;
    [SerializeField] private float damageAmnt;
    public bool isFrightened;
    public bool isFollowing;
    public List<GameObject> monsters;

    void Awake(){
        monsters = new List<GameObject>();
    }

    public void Hit(){

        if(health > 0){
            TakeDamage(damageAmnt);
        } else{
            OnDeath();
        }
    }
    public void TakeDamage(float damageAmount){
        health -= damageAmount;
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    void OnDeath(){
        //remove references to human in all following monsters
        for(int i = 0; i < monsters.Count; i++){
            monsters[i].GetComponent<EnemyBehavior>().Unlink();
        }

        Destroy(this.gameObject);
    }
}
