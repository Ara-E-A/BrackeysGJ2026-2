using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    [SerializeField] private Talker talker;
    private TextMeshProUGUI textField;

    void Start()
    {
        getTextField();
        if (talker == null)
        {
            talker = FindAnyObjectByType<Talker>();
        }
        this.gameObject.SetActive(false);
    }

    public void showEvent(string eventText)
    {
        this.gameObject.SetActive(true);
        DBoxControl.WakeyWakey();

        if (eventText != null && textField != null)
        {
            textField.text = eventText;
        }

    }

    public void showEvent(InteractEvent interactEvent)
    {
        if (interactEvent == null)
        {
            return;
        }

        showEvent(interactEvent.eventText);
        if (talker != null)
        {
            talker.CreateButtons(interactEvent.options);
        }
    }


    public void showEvent(string eventText, Image eventImg)
    {
        this.gameObject.SetActive(true);

    }

    public void unshowEvent()
    {
        if (talker != null)
        {
            talker.WipeButtons();
        }
        DBoxControl.stopSpeaking();
        this.gameObject.SetActive(false);
    }

    public void getTextField()
    {
        this.textField = this.GetComponentInChildren<TextMeshProUGUI>();
    }
}
