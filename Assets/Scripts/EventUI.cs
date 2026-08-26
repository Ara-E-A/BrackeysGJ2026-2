using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class EventUI : MonoBehaviour
{
    private TextMeshProUGUI textField;

    void Start()
    {
        getTextField();
        this.gameObject.SetActive(false);
    }

    public void showEvent(string eventText)
    {
        this.gameObject.SetActive(true);

        if(eventText != null)
        {
            textField.text = eventText;
        }

    }


    public void showEvent(string eventText, Image eventImg)
    {
        this.gameObject.SetActive(true);

    }

    public void unshowEvent()
    {
        this.gameObject.SetActive(false);
    }

    public void getTextField()
    {
        this.textField = this.GetComponentInChildren<TextMeshProUGUI>();
    }
}
