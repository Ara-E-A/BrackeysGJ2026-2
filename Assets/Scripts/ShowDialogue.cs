using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ShowDialogue : MonoBehaviour
{

    public GameObject diaBox;

    public float typingDelay = 0.05f;
    
    private TextMeshProUGUI textField;
    private Coroutine typingCoroutine;

    public void Start()
    {
        getTextField();
    }

    public void showDialogue(string dialogue)
    {
        if(this.textField != null)
        {
            if (this.typingCoroutine != null)
            {
                StopCoroutine(this.typingCoroutine);
            }

            this.typingCoroutine = StartCoroutine(typeDialogue(dialogue));
        }
    }

    private IEnumerator typeDialogue(string dialogue)
    {
        this.textField.text = string.Empty;

        foreach (char character in dialogue)
        {
            this.textField.text += character;
            yield return new WaitForSeconds(this.typingDelay);
        }
        this.typingCoroutine = null;
    }

    public void getTextField()
    {
        this.textField = this.GetComponentInParent<TextMeshProUGUI>();

        if (this.textField != null) 
        {
            Debug.Log("Got the text thingy.");
            showDialogue("When the Bingus is Schmungus.");
        }
    }
}
