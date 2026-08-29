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

    [SerializeField] private Talker talker;

    private TextMeshProUGUI textField;
    private Coroutine typingCoroutine;

    public void Start()
    {
        getTextField();
        if (talker == null)
        {
            talker = FindAnyObjectByType<Talker>();
        }
    }

    public void Update()
    {
        if(!DBoxControl.speaking && textField != null)
        {
            textField.text = "";
        }
    }

    public void ShowNPCLine(string text, params InteractEventOption[] choices)
    {
        DBoxControl.WakeyWakey();
        showDialogue(text);

        if (choices == null || choices.Length < 1)
        {
            return;
        }

        if (talker == null)
        {
            talker = FindAnyObjectByType<Talker>();
        }

        if (talker == null)
        {
            Debug.LogError("ShowDialogue: no Talker found to build choice buttons.");
            return;
        }

        talker.CreateButtons(choices);
    }

    public void EndNPCLine()
    {
        if (talker != null)
        {
            talker.WipeButtons();
        }

        DBoxControl.stopSpeaking();
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
        // The dialogue text now lives on a dedicated "DialogueText" child that renders
        // above the frame; fall back to a child search if it was renamed.
        Transform textTransform = transform.Find("DialogueText");
        this.textField = textTransform != null
            ? textTransform.GetComponent<TextMeshProUGUI>()
            : GetComponentInChildren<TextMeshProUGUI>(true);

        if (this.textField != null)
        {
            showDialogue("");
        }
    }
}
