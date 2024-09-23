using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TMP_Text textMesh;
    public float typingSpeed = 0.1f;
    private string fullText;

    private void Awake(){
        fullText = textMesh.text;
        textMesh.text = string.Empty;
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText(){
        foreach(char letter in fullText){
            textMesh.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
