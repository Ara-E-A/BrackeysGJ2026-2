using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ShowDialogue : MonoBehaviour
{

    public GameObject diaBox;
    
    private TextMeshProUGUI textField;

    public void Start()
    {
        getTextField();
    }

    public void showDialogue(string dialogue)
    {
        if(this.textField != null)
        {
            this.textField.text = dialogue;
        }
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
