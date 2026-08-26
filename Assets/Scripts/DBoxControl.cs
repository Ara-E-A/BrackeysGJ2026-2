using System.Collections;
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
        StartCoroutine(fadeOutAfterDelay());
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
            StartCoroutine(fadeOutAfterDelay());
        }
    }

    private IEnumerator fadeOutAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (!speaking)
        {
            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                dialogGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                yield return null;
            }

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
