using TMPro;
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
        Transform textTransform = transform.Find("DialogueText");
        this.dialog = textTransform != null
            ? textTransform.GetComponent<TextMeshProUGUI>()
            : GetComponentInChildren<TextMeshProUGUI>(true);
        this.dialogOptionsBox = this.dialog;
        this.dialogGroup = GetComponent<CanvasGroup>();
        if (this.dialogGroup == null)
        {
            this.dialogGroup = gameObject.AddComponent<CanvasGroup>();
        }
        fadeOut();
    }

    public void Update()
    {
        // Gate UI raycasts with the dialogue state: while speaking the whole dialogue
        // box (Panel / options / close button) blocks clicks; when closed it lets them
        // through so world raycasting is normal again.
        if (dialogGroup != null)
        {
            dialogGroup.blocksRaycasts = speaking;
        }

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
