using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Full-screen win/lose overlay that mimics the intro screen: a black pane with typed text
/// that fades out after the message finishes.
/// </summary>
public class EndScreenUI : MonoBehaviour
{
    [Header("Text")]
    [TextArea(2, 6)]
    [SerializeField] private string winText = "You win.";
    [TextArea(2, 6)]
    [SerializeField] private string loseText = "You lose.";

    [Header("Timing")]
    [SerializeField] private float typeDelay = 0.04f;
    [SerializeField] private float pauseAfterTyping = 0.8f;
    [SerializeField] private float fadeDuration = 0.7f;

    [Header("Appearance")]
    [SerializeField] private int fontSize = 54;
    [SerializeField] private Color textColor = Color.white;

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textComponent;
    private Image backgroundImage;
    private Coroutine endRoutine;

    private void Awake()
    {
        BuildOverlay();
        gameObject.SetActive(false);
    }

    private void BuildOverlay()
    {
        Canvas targetCanvas = FindAnyObjectByType<Canvas>();
        if (targetCanvas == null)
        {
            GameObject canvasObject = new GameObject("EndCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            targetCanvas = canvasObject.GetComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            targetCanvas.sortingOrder = 600;

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

        GameObject textObject = transform.Find("EndText")?.gameObject;
        if (textObject == null)
        {
            textObject = new GameObject("EndText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(transform, false);
        }

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.15f);
        textRect.anchorMax = new Vector2(0.9f, 0.85f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        textComponent = textObject.GetComponent<TextMeshProUGUI>();
        textComponent.fontSize = fontSize;
        textComponent.color = textColor;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.textWrappingMode = TextWrappingModes.Normal;
        textComponent.raycastTarget = false;
        textComponent.alpha = 1f;
        textComponent.maxVisibleCharacters = 0;
    }

    public void Show(bool win)
    {
        if (endRoutine != null)
        {
            StopCoroutine(endRoutine);
        }

        string message = win ? winText : loseText;
        gameObject.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        if (textComponent != null)
        {
            textComponent.text = message;
            textComponent.alpha = 1f;
            textComponent.maxVisibleCharacters = 0;
        }

        endRoutine = StartCoroutine(PlayEndRoutine(message));
    }

    public void Hide()
    {
        if (endRoutine != null)
        {
            StopCoroutine(endRoutine);
            endRoutine = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (textComponent != null)
        {
            textComponent.text = string.Empty;
            textComponent.maxVisibleCharacters = 0;
            textComponent.alpha = 0f;
        }

        gameObject.SetActive(false);
    }

    private IEnumerator PlayEndRoutine(string message)
    {
        if (textComponent == null || canvasGroup == null)
        {
            yield break;
        }

        textComponent.text = message;
        textComponent.maxVisibleCharacters = 0;

        int totalChars = message.Length;
        while (textComponent.maxVisibleCharacters < totalChars)
        {
            textComponent.maxVisibleCharacters += 1;
            yield return new WaitForSeconds(typeDelay);
        }

        yield return new WaitForSeconds(pauseAfterTyping);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(1f, 0f, t);
            canvasGroup.alpha = alpha;
            textComponent.alpha = alpha;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        textComponent.alpha = 0f;
        endRoutine = null;
        gameObject.SetActive(false);
    }
}
