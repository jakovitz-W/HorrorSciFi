using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public GameObject hintText, diContainer, diText;
    public Image containerImg;
    public List<Dialogue> diList; //add to list in inspector to create dialogue instances
    public List<Dialogue> hintList;
    private bool hint;

    void Awake(){
        hintText.SetActive(false);
        diContainer.SetActive(false);
    }

    //find the index of dialogue instance with specified key, if key does not exist it returns -1
    public int FindIndexByKey(string searchKey){
        
        if(!hint){
            for(int i = 0; i < diList.Count; i++){ //using List.Count in this case is okay because the loop breaks at i = List.Count
                if(diList[i].GetKey() == searchKey){
                    return i;
                }
            }            
        } else{
            for(int i = 0; i < hintList.Count; i++){
                if(hintList[i].GetKey() == searchKey){
                    return i;
                }
            }     
        }

        return -1; //error
    }

    //isHint to specify if the text should be hint text or a popup dialogue box
    public void SetText(string key, bool isHint, bool random, int specific){
        hint = isHint;
        int index = FindIndexByKey(key);
        if(hint){
            hintText.SetActive(true);
            hintText.GetComponent<TypewriterEffect>().SetString(hintList[index].GetDialogue(specific), true);
        } else{
            
            diContainer.SetActive(true);

            containerImg.sprite = diList[index].characterImage;
            if(!random){
                diText.GetComponent<TypewriterEffect>().SetString(diList[index].GetDialogue(specific), false);
            } else{
                //add check for removal, set to false for now
                diText.GetComponent<TypewriterEffect>().SetString(diList[index].GetRandomOption(false), false);
            }
        }
    }
}

[System.Serializable]
public class Dialogue{ //set fields in inspector
    [SerializeField] private string key;
    [SerializeField] private List<string> di; //using list for easy removal
    private int iteration;
    public Sprite characterImage;

    void Awake(){
        iteration = 0;
    }

    public string GetKey(){
        return key;
    }

    //checks if next option exists & picks from list accordingly || for ordered dialogue
    public string GetDialogue(int specific){
        string option;

        if(specific != -1){
            option = di[specific];
            return option;
        }
        option = di[iteration];

        if(iteration < (di.Count - 1)){ //.count starts counting at 1, index for lists start at 0
            iteration++;
        }

        return option;
    }

    //picks random option from string array with or without removal|| for unordered dialogue
    public string GetRandomOption(bool remove){

        int rand = Random.Range(0, (di.Count));
        string option = di[rand];

        if(remove){
            di.Remove(option);
        }
        return option;
    }
}