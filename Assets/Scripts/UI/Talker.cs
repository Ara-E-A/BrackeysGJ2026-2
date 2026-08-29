using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Talker : MonoBehaviour
{
    [SerializeField] private Vector2 buttonSize = new Vector2(80f, 20f);
    [SerializeField] private Vector2 spacing = new Vector2(16f, 16f);
    private readonly List<Button> buttons = new List<Button>();

    void Update()
    {
        if (!DBoxControl.speaking)
        {
            WipeButtons();
        }
    }

    public void WipeButtons()
    {
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        buttons.Clear();
    }

    public void CreateButtons(params InteractEventOption[] buttonOptions)
    {
        if (buttonOptions == null || buttonOptions.Length < 1 || buttonOptions.Length > 4)
        {
            Debug.LogError("Talker requires between 1 and 4 buttons.");
            return;
        }

        WipeButtons();

        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            grid = gameObject.AddComponent<GridLayoutGroup>();
        }

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 1;
        grid.cellSize = buttonSize;
        grid.spacing = spacing;
        transform.SetAsLastSibling();

        for (int index = 0; index < buttonOptions.Length; index++)
        {
            GameObject buttonObject = new GameObject($"DialogButton{index + 1}", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(transform, false);
            buttonObject.transform.SetAsLastSibling();
            Button button = buttonObject.GetComponent<Button>();
            buttons.Add(button);
            InteractEventOption option = buttonOptions[index];
            if (option != null && option.EventAction != null)
                button.onClick.AddListener(option.EventAction.Invoke);

            TextMeshProUGUI label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            label.transform.SetParent(buttonObject.transform, false);
            label.text = option == null ? string.Empty : option.label;
            label.fontSize = 14;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.black;

            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
        }
    }
}
