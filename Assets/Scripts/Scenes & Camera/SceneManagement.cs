using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [SerializeField] GameObject credits;
    public void StartGame(){

    }

    public void QuitGame(){
        Debug.Log("Quit");
        Application.Quit();
    }

    public void Credits(){
        credits.SetActive(true);
    }
}
