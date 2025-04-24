using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    [SerializeField] private GameObject[] crates;
    private List<Vector2> crateOrigins;

    void OnEnable(){
        if(crateOrigins == null){
            crateOrigins = new List<Vector2>();
            for(int i = 0; i < crates.Length; i++){
                crateOrigins.Add(crates[i].transform.position);
            }            
        }
    }

    public void Reset(){
        for(int i = 0; i < crates.Length; i++){
            crates[i].transform.position = crateOrigins[i];
        }
    }
}
