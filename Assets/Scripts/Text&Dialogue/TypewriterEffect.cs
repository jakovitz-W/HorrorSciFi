using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TypewriterEffect : MonoBehaviour
{
    private PlayerControls controls;
    public TMP_Text textMesh;
    public float defaultSpeed = 0.1f;
    private float typingSpeed;
    private string fullText;
    private bool hint;
    private bool allowHint = true;

    public void DisableHints(){
        allowHint = false;
    }

    public void SetTextSpeed(float speed){
        //have to do it this way bc unity does not allow min to be larger than max on sliders
        //in this case the larger values equal slower text speed, so it needs to be reversed
        defaultSpeed = Mathf.Abs(speed);
        typingSpeed = defaultSpeed;
    }

    private void Awake(){
        fullText = textMesh.text;
        textMesh.text = string.Empty;
        controls = new PlayerControls();
        typingSpeed = defaultSpeed;
    }

    void OnEnable(){
        controls.General.Skip.performed += SkipDialogue;
        controls.Enable();
    }

    void OnDisable(){
        controls.General.Skip.performed -= SkipDialogue;
        controls.Disable();
    }

    public void SetString(string text, bool isHint){
        hint = isHint;

        if(isHint && !allowHint){
            return;
        }

        fullText = text;
        textMesh.text = string.Empty;
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText(){
        foreach(char letter in fullText){
            textMesh.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        if(hint){
            StartCoroutine(FadeText());              
        }
          
    }

    IEnumerator FadeText(){
        yield return new WaitForSeconds(2);
        this.gameObject.SetActive(false);
    }

    void SkipDialogue(InputAction.CallbackContext ctx){

        if(ctx.performed){
            if(!hint){

                if(textMesh.text != fullText){
                    typingSpeed = 0.001f;
                } else{
                    GameObject container = this.gameObject.transform.parent.gameObject;
                    textMesh.text = string.Empty;
                    typingSpeed = defaultSpeed;
                    container.SetActive(false);
                }
            }
        }
    }
}
