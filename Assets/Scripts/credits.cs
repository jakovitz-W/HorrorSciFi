using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class credits : MonoBehaviour
{
    public float scrollspeed = 5f;
    private Vector2 start;
    public Transform last, threshold;
    public GameObject parent;
    void OnEnable(){
        start = transform.position;
        AudioManager.Instance.PlayMusic("boss");
    }

    void Update(){
        Vector2 targetPos = new Vector2(transform.position.x, transform.position.y + 1);
        float step = scrollspeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, step);

        if(last.position.y >= threshold.position.y){
            Reset();
            parent.SetActive(false);
        }
    }

    public void Reset(){
        AudioManager.Instance.StopAll();
        transform.position = start;
    }
}
