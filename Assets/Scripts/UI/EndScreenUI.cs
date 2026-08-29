using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the end-of-game overlay in the camera HUD. It activates its root object,
/// switches between the Win and Lose child panels, and slides itself into view from below.
/// </summary>
public class EndScreenUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private RectTransform winPanel;
    [SerializeField] private RectTransform losePanel;

    [Header("Animation")]
    [SerializeField] private float slideDuration = 0.6f;
    [SerializeField] private Vector2 visibleAnchoredPosition = Vector2.zero;
    [SerializeField] private Vector2 hiddenAnchoredPosition = new Vector2(0f, -500f);

    private RectTransform rootRect;
    private CanvasGroup canvasGroup;
    private Coroutine slideCoroutine;

    private void Awake()
    {
        rootRect = GetComponent<RectTransform>();
        if (rootRect == null)
        {
            rootRect = transform as RectTransform;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (rootRect != null)
        {
            rootRect.anchoredPosition = hiddenAnchoredPosition;
        }

        SetPanelState(true, false);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void OnEnable()
    {
        if (rootRect != null)
        {
            rootRect.anchoredPosition = hiddenAnchoredPosition;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    /// <summary>
    /// Activates the end-screen and shows the win or lose panel, then slides the HUD into view.
    /// True is win, False is lose
    /// </summary>
    public void Show(bool win)
    {
        gameObject.SetActive(true);

        SetPanelState(win, !win);

        if (rootRect == null)
        {
            rootRect = GetComponent<RectTransform>();
        }

        if (rootRect == null)
        {
            return;
        }

        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }

        slideCoroutine = StartCoroutine(SlideIn());
    }

    public void Hide()
    {
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
            slideCoroutine = null;
        }

        if (rootRect != null)
        {
            rootRect.anchoredPosition = hiddenAnchoredPosition;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        gameObject.SetActive(false);
    }

    private void SetPanelState(bool showWin, bool showLose)
    {
        if (winPanel != null)
        {
            winPanel.gameObject.SetActive(showWin);
        }

        if (losePanel != null)
        {
            losePanel.gameObject.SetActive(showLose);
        }
    }

    private IEnumerator SlideIn()
    {
        if (rootRect == null)
        {
            yield break;
        }

        Vector2 startPosition = hiddenAnchoredPosition;
        Vector2 endPosition = visibleAnchoredPosition;
        rootRect.anchoredPosition = startPosition;

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / slideDuration);
            float eased = Mathf.SmoothStep(0f, 1f, progress);

            rootRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, eased);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, eased);
            }

            yield return null;
        }

        rootRect.anchoredPosition = endPosition;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        slideCoroutine = null;
    }
}
