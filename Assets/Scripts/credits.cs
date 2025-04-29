using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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
            Cursor.visible = true;
            AudioManager.Instance.StopAll();
            SceneManager.LoadScene(0); //replace w/ call to scenemanagement.loadasync
            parent.SetActive(false);
        }
    }

    public void Reset(){
        AudioManager.Instance.StopAll();
        transform.position = start;
    }
}
