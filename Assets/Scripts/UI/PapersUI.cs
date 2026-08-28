using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Tab (or Esc to close) opens the traveller's paper into a focused, editable form showing
/// all six <see cref="PlayerPaper"/> identifiers as runtime-built widgets bound live to
/// <see cref="GameManager.playerPaper"/>.
///
/// Lives on the scene's <c>Papers</c> GameObject. Reuses <see cref="PaperHUD"/> for the
/// zoom animation and self-wires everything else (re-anchors <c>PapersOpen</c>, adds the
/// layout group, disables the old <c>PapersOpen</c> button) at runtime.
///
/// Opening while an NPC dialogue is up ends that dialogue (same as picking "Leave"), and
/// world interaction is suppressed while the paper is open (see <see cref="Interactor"/>
/// and <see cref="DialogManager"/>).
/// </summary>
public class PapersUI : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    // Height is fixed at 120-230 cm and enforced, not designer-tunable.
    private const int HeightMin = 120;
    private const int HeightMax = 230;

    [Header("Field caps")]
    [SerializeField] private int nameMaxLength = 20;
    [SerializeField] private int originMaxLength = 20;
    [SerializeField] private int ageMin = 0;
    [SerializeField] private int ageMax = 108;
    [SerializeField] private int idMin = 0;
    [SerializeField] private int idMax = 9999;

    private PaperHUD paperHUD;
    private RectTransform papersOpen;
    private Button papersOpenButton;
    private TextMeshProUGUI closedText;
    private CameraLook cameraLook;

    private GameObject fieldsRoot;
    private bool built;

    private PlayerPaper paper;

    private TMP_InputField nameField;
    private TMP_InputField originField;
    private TMP_Dropdown sexField;
    private TMP_InputField idField;
    private TMP_InputField heightField;
    private TMP_InputField ageField;

    private void Awake()
    {
        paperHUD = GetComponent<PaperHUD>();

        Transform open = transform.Find("PapersOpen");
        if (open != null)
        {
            papersOpen = open as RectTransform;
            papersOpenButton = open.GetComponent<Button>();
        }

        Transform text = transform.Find("PaperText");
        if (text != null)
        {
            closedText = text.GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnDisable()
    {
        IsOpen = false;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.tabKey.wasPressedThisFrame)
        {
            Toggle();
        }
        else if (IsOpen && keyboard.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    public void Toggle()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        if (IsOpen)
        {
            return;
        }

        // Opening over a live dialogue dismisses it, same as choosing "Leave".
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.CloseActiveDialogue();
        }

        // Also clears any non-DialogManager dialogue (e.g. the Inspector): wipes the
        // choice buttons and resets DBoxControl so interaction is not left soft-locked.
        ShowDialogue openDialogue = FindAnyObjectByType<ShowDialogue>();
        if (openDialogue != null)
        {
            openDialogue.EndNPCLine();
        }

        GameManager gm = FindAnyObjectByType<GameManager>();
        paper = gm != null ? gm.playerPaper : null;
        if (paper == null)
        {
            Debug.LogWarning("PapersUI: no GameManager / PlayerPaper found; fields will be blank.");
        }

        BuildFieldsOnce();
        RefreshFromPaper();

        if (fieldsRoot != null)
        {
            fieldsRoot.SetActive(true);
        }

        if (papersOpenButton != null)
        {
            papersOpenButton.enabled = false;
        }

        if (paperHUD != null)
        {
            paperHUD.Open();
        }

        IsOpen = true;
    }

    public void Close()
    {
        if (!IsOpen)
        {
            return;
        }

        if (fieldsRoot != null)
        {
            fieldsRoot.SetActive(false);
        }

        if (paperHUD != null)
        {
            paperHUD.Close();
        }

        if (closedText != null && paper != null)
        {
            closedText.text = paper.name;
        }

        IsOpen = false;
    }

    // ---------------- field <-> paper ----------------

    private void RefreshFromPaper()
    {
        if (paper == null || !built)
        {
            return;
        }

        nameField.SetTextWithoutNotify(paper.name ?? string.Empty);
        originField.SetTextWithoutNotify(paper.origin ?? string.Empty);
        idField.SetTextWithoutNotify(Mathf.Clamp(Mathf.RoundToInt(paper.id), idMin, idMax).ToString());

        // Correct any out-of-range height on the paper itself so it can never be submitted,
        // then align the camera with it.
        int height = Mathf.Clamp(Mathf.RoundToInt(paper.height), HeightMin, HeightMax);
        paper.height = height;
        heightField.SetTextWithoutNotify(height.ToString());
        UpdateCameraHeight();

        ageField.SetTextWithoutNotify(Mathf.RoundToInt(paper.age).ToString());

        int sexIndex = Array.IndexOf(PlayerPaper.AllSexes, paper.sex);
        sexField.SetValueWithoutNotify(sexIndex < 0 ? 0 : sexIndex);
    }

    // Smoothly slide the camera to the (already-clamped) paper height.
    private void UpdateCameraHeight()
    {
        if (paper == null)
        {
            return;
        }

        if (cameraLook == null)
        {
            cameraLook = FindAnyObjectByType<CameraLook>();
        }

        if (cameraLook != null)
        {
            cameraLook.SetCameraHeightFromPaper(paper);
        }
    }

    // ---------------- runtime UI construction ----------------

    private void BuildFieldsOnce()
    {
        if (built)
        {
            return;
        }

        built = true;

        RectTransform parent = papersOpen != null ? papersOpen : (RectTransform)transform;

        fieldsRoot = new GameObject("Fields", typeof(RectTransform), typeof(VerticalLayoutGroup));
        RectTransform rootRect = (RectTransform)fieldsRoot.transform;
        rootRect.SetParent(parent, false);
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = new Vector2(10f, 10f);
        rootRect.offsetMax = new Vector2(-10f, -10f);

        VerticalLayoutGroup layout = fieldsRoot.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 6f;
        layout.padding = new RectOffset(6, 6, 6, 6);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.childAlignment = TextAnchor.UpperCenter;

        nameField = AddInputRow("Name");
        nameField.characterLimit = nameMaxLength;
        nameField.contentType = TMP_InputField.ContentType.Standard;
        nameField.onValidateInput = LettersOnly;
        nameField.onValueChanged.AddListener(value =>
        {
            if (paper != null) paper.name = value;
        });

        originField = AddInputRow("Origin");
        originField.characterLimit = originMaxLength;
        originField.contentType = TMP_InputField.ContentType.Standard;
        originField.onValidateInput = LettersOnly;
        originField.onValueChanged.AddListener(value =>
        {
            if (paper != null) paper.origin = value;
        });

        sexField = AddDropdownRow("Sex");
        sexField.ClearOptions();
        sexField.AddOptions(new List<string>(PlayerPaper.AllSexes));
        sexField.onValueChanged.AddListener(index =>
        {
            if (paper != null && index >= 0 && index < PlayerPaper.AllSexes.Length)
            {
                paper.sex = PlayerPaper.AllSexes[index];
            }
        });

        idField = AddInputRow("ID");
        idField.contentType = TMP_InputField.ContentType.IntegerNumber;
        idField.characterLimit = 4;
        idField.onEndEdit.AddListener(value =>
        {
            int.TryParse(value, out int parsed);
            int clamped = Mathf.Clamp(parsed, idMin, idMax);
            if (paper != null) paper.id = clamped;
            idField.SetTextWithoutNotify(clamped.ToString());
        });

        heightField = AddInputRow("Height");
        heightField.contentType = TMP_InputField.ContentType.IntegerNumber;
        heightField.characterLimit = 3;
        heightField.onValidateInput = HeightValidate;
        heightField.onValueChanged.AddListener(value =>
        {
            // Live: validate -> clamp -> store -> move the camera as the player types.
            int.TryParse(value, out int parsed);
            int clamped = Mathf.Clamp(parsed, HeightMin, HeightMax);
            if (paper != null) paper.height = clamped;
            UpdateCameraHeight();
        });
        heightField.onEndEdit.AddListener(value =>
        {
            int.TryParse(value, out int parsed);
            int clamped = Mathf.Clamp(parsed, HeightMin, HeightMax);
            if (paper != null) paper.height = clamped;
            heightField.SetTextWithoutNotify(clamped.ToString());
            UpdateCameraHeight();
        });

        ageField = AddInputRow("Age");
        ageField.contentType = TMP_InputField.ContentType.IntegerNumber;
        ageField.onEndEdit.AddListener(value =>
        {
            int.TryParse(value, out int parsed);
            int clamped = Mathf.Clamp(parsed, ageMin, ageMax);
            if (paper != null) paper.age = clamped;
            ageField.SetTextWithoutNotify(clamped.ToString());
        });
    }

    private static char LettersOnly(string text, int charIndex, char addedChar)
    {
        return char.IsLetter(addedChar) ? addedChar : '\0';
    }

    // Digits only, and rejects any keystroke that would push the value above HeightMax.
    // (The 120 minimum is enforced by the end-edit clamp - you must be able to type "1", "12"...)
    private static char HeightValidate(string text, int charIndex, char addedChar)
    {
        if (!char.IsDigit(addedChar))
        {
            return '\0';
        }

        string prospective = text.Substring(0, charIndex) + addedChar + text.Substring(charIndex);
        if (int.TryParse(prospective, out int value) && value > HeightMax)
        {
            return '\0';
        }

        return addedChar;
    }

    private GameObject NewRow(string label)
    {
        GameObject row = new GameObject($"Row_{label}", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        row.transform.SetParent(fieldsRoot.transform, false);

        HorizontalLayoutGroup hlg = row.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8f;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.childAlignment = TextAnchor.MiddleLeft;

        LayoutElement rowElement = row.GetComponent<LayoutElement>();
        rowElement.minHeight = 30f;
        rowElement.preferredHeight = 30f;

        GameObject labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
        labelGO.transform.SetParent(row.transform, false);
        TextMeshProUGUI labelText = labelGO.GetComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 14f;
        labelText.color = Color.black;
        labelText.alignment = TextAlignmentOptions.MidlineLeft;
        labelGO.GetComponent<LayoutElement>().preferredWidth = 70f;

        return row;
    }

    private TMP_InputField AddInputRow(string label)
    {
        GameObject row = NewRow(label);

        GameObject fieldGO = TMP_DefaultControls.CreateInputField(GetResources());
        fieldGO.name = $"Input_{label}";
        fieldGO.transform.SetParent(row.transform, false);
        fieldGO.AddComponent<LayoutElement>().flexibleWidth = 1f;

        if (fieldGO.TryGetComponent(out Image background))
        {
            background.color = new Color(0.93f, 0.93f, 0.93f, 1f);
        }

        TMP_InputField input = fieldGO.GetComponent<TMP_InputField>();
        input.pointSize = 14f;
        if (input.textComponent != null)
        {
            input.textComponent.color = Color.black;
            input.textComponent.fontSize = 14f;
        }
        if (input.placeholder is TextMeshProUGUI placeholder)
        {
            placeholder.text = string.Empty;
        }

        return input;
    }

    private TMP_Dropdown AddDropdownRow(string label)
    {
        GameObject row = NewRow(label);

        GameObject dropdownGO = TMP_DefaultControls.CreateDropdown(GetResources());
        dropdownGO.name = $"Dropdown_{label}";
        dropdownGO.transform.SetParent(row.transform, false);
        dropdownGO.AddComponent<LayoutElement>().flexibleWidth = 1f;

        if (dropdownGO.TryGetComponent(out Image background))
        {
            background.color = new Color(0.93f, 0.93f, 0.93f, 1f);
        }

        Transform arrow = dropdownGO.transform.Find("Arrow");
        if (arrow != null && arrow.TryGetComponent(out Image arrowImage))
        {
            arrowImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        TMP_Dropdown dropdown = dropdownGO.GetComponent<TMP_Dropdown>();
        if (dropdown.captionText != null)
        {
            dropdown.captionText.color = Color.black;
            dropdown.captionText.fontSize = 14f;
        }
        if (dropdown.itemText != null)
        {
            dropdown.itemText.color = Color.black;
            dropdown.itemText.fontSize = 14f;
        }

        return dropdown;
    }

    private bool resourcesReady;
    private TMP_DefaultControls.Resources cachedResources;
    private static Sprite runtimeSprite;

    private TMP_DefaultControls.Resources GetResources()
    {
        if (resourcesReady)
        {
            return cachedResources;
        }

        Sprite sprite = RuntimeSprite();
        cachedResources = new TMP_DefaultControls.Resources
        {
            standard = sprite,
            background = sprite,
            inputField = sprite,
            knob = sprite,
            checkmark = sprite,
            dropdown = sprite,
            mask = sprite,
        };
        resourcesReady = true;
        return cachedResources;
    }

    /// <summary>
    /// A white sprite built from Unity's always-available built-in white texture. Used
    /// instead of <c>Resources.GetBuiltinResource&lt;Sprite&gt;("UI/Skin/*.psd")</c>, whose
    /// paths are not valid in this Unity version and spam "Failed to find UI/Skin/*.psd".
    /// The widgets are tinted per-Image below, so a plain white source sprite is enough.
    /// </summary>
    private static Sprite RuntimeSprite()
    {
        if (runtimeSprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            runtimeSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            runtimeSprite.name = "PapersUI_White";
        }

        return runtimeSprite;
    }
}
