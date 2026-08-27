using TMPro;
using UnityEditor;
using UnityEngine;

public class DBoxControl : MonoBehaviour
{
    public TextMeshProUGUI dialog;
    public TextMeshProUGUI dialogOptionsBox;
    private CanvasGroup dialogGroup;
    public static bool speaking = false;
    public static bool fading = true;

    public void Start()
    {
        this.dialog = GetComponent<TextMeshProUGUI>();
        this.dialogOptionsBox = GetComponentInChildren<TextMeshProUGUI>();
        this.dialogGroup = GetComponent<CanvasGroup>();
        if (this.dialogGroup == null)
        {
            this.dialogGroup = gameObject.AddComponent<CanvasGroup>();
        }
        fadeOut();
    }

    public void Update()
    {
        if(speaking)
        {
            fading = false;
            dialogGroup.alpha = 1f;
            dialog.alpha = 1f;
        }
        else if (!fading && !speaking)
        {
            fading = true;
            fadeOut();
        }
    }

    private void fadeOut()
    {
        if (!speaking)
        {
            dialogGroup.alpha = 0f;
            dialog.alpha = 0f;
        }
    }

    public static void stopSpeaking()
    {
        speaking = false;
        Debug.Log("Stopped Speaking");
    }

    public static void WakeyWakey()
    {
        speaking = true;
    }
}
