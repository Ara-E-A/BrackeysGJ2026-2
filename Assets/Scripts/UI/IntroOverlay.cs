using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class IntroOverlay : MonoBehaviour
{
    [Header("Text")]
    [TextArea(2, 6)]
    public string introText = "Welcome to the game...";

    [Header("Timing")]
    [SerializeField] private float typeDelay = 0.04f;
    [SerializeField] private float pauseAfterTyping = 0.75f;
    [SerializeField] private float fadeDuration = 0.7f;

    [Header("Appearance")]
    [SerializeField] private int fontSize = 42;
    [SerializeField] private Color textColor = Color.white;

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textComponent;
    private Image backgroundImage;
    private Coroutine introRoutine;
    private bool skipRequested;

    private void Awake()
    {
        BuildOverlay();
        introRoutine = StartCoroutine(PlayIntroRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && introRoutine != null)
        {
            SkipIntro();
        }
    }

    private void BuildOverlay()
    {
        if (textComponent != null && canvasGroup != null)
        {
            return;
        }

        Canvas targetCanvas = FindAnyObjectByType<Canvas>();
        if (targetCanvas == null)
        {
            GameObject canvasObject = new GameObject("IntroCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            targetCanvas = canvasObject.GetComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            targetCanvas.sortingOrder = 500;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        transform.SetParent(targetCanvas.transform, false);

        RectTransform rootRect = GetComponent<RectTransform>();
        if (rootRect == null)
        {
            rootRect = gameObject.AddComponent<RectTransform>();
        }

        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }

        if (backgroundImage == null)
        {
            backgroundImage = gameObject.AddComponent<Image>();
        }

        backgroundImage.color = new Color(0f, 0f, 0f, 1f);
        backgroundImage.raycastTarget = false;

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        GameObject textObject = transform.Find("IntroText")?.gameObject;
        if (textObject == null)
        {
            textObject = new GameObject("IntroText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(transform, false);
        }

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.15f);
        textRect.anchorMax = new Vector2(0.9f, 0.85f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        textComponent = textObject.GetComponent<TextMeshProUGUI>();
        textComponent.text = introText;
        textComponent.fontSize = fontSize;
        textComponent.color = textColor;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.textWrappingMode = TextWrappingModes.Normal;
        textComponent.raycastTarget = false;
        textComponent.alpha = 1f;
        textComponent.maxVisibleCharacters = 0;
    }

    public void SetText(string newText)
    {
        introText = newText;
        if (textComponent != null)
        {
            textComponent.text = introText;
            textComponent.maxVisibleCharacters = 0;
        }
    }

    public void SkipIntro()
    {
        if (skipRequested)
        {
            return;
        }

        skipRequested = true;

        if (introRoutine != null)
        {
            StopCoroutine(introRoutine);
            introRoutine = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (textComponent != null)
        {
            textComponent.alpha = 0f;
            textComponent.maxVisibleCharacters = introText.Length;
        }

        Destroy(gameObject, 0.1f);
    }

    private IEnumerator PlayIntroRoutine()
    {
        if (canvasGroup == null || textComponent == null)
        {
            yield break;
        }

        if (skipRequested)
        {
            yield break;
        }

        int totalChars = introText.Length;

        while (textComponent.maxVisibleCharacters < totalChars && !skipRequested)
        {
            textComponent.maxVisibleCharacters += 1;
            yield return new WaitForSeconds(typeDelay);
        }

        if (skipRequested)
        {
            yield break;
        }

        yield return new WaitForSeconds(pauseAfterTyping);

        if (skipRequested)
        {
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration && !skipRequested)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(1f, 0f, t);
            canvasGroup.alpha = alpha;
            textComponent.alpha = alpha;
            yield return null;
        }

        if (skipRequested)
        {
            yield break;
        }

        canvasGroup.alpha = 0f;
        textComponent.alpha = 0f;

        Destroy(gameObject, 0.1f);
    }
}
