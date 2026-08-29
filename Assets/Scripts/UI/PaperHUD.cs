using System.Collections;
using UnityEngine;

/// <summary>
/// Zoom animation for the traveller's paper, driven by <see cref="PapersUI"/>:
/// <see cref="Open"/> moves the paper into a centred focus position and scales it up,
/// <see cref="Close"/> returns it to its resting corner.
///
/// Both states are animated in <see cref="RectTransform.anchoredPosition"/> /
/// <see cref="RectTransform.sizeDelta"/> space - raw serialized layout data that is valid
/// in <c>Awake</c> and independent of canvas scale - so opening/closing never shifts the
/// paper to a different part of the screen.
/// </summary>
public class PaperHUD : MonoBehaviour
{
	[SerializeField] private RectTransform paper;
	[SerializeField] private float expandedScale = 2.5f;
	[SerializeField] private float animationDuration = 0.35f;

	[Tooltip("anchoredPosition the paper animates to when opened (its focused/zoomed spot). " +
	         "Default (0,0) centres it on the canvas.")]
	[SerializeField] private Vector2 openAnchoredPosition = Vector2.zero;

	private Vector2 closedAnchoredPosition;
	private Vector2 closedSize;
	private Coroutine animationCoroutine;

	private void Awake()
	{
		if (paper == null)
		{
			paper = GetComponent<RectTransform>();
		}

		if (paper != null)
		{
			closedAnchoredPosition = paper.anchoredPosition;
			closedSize = paper.sizeDelta;
		}
	}

	/// <summary>Zoom the paper into its focused position.</summary>
	public void Open()
	{
		if (paper == null)
		{
			return;
		}

		StartAnimation(openAnchoredPosition, closedSize * expandedScale);
	}

	/// <summary>Zoom the paper back to its resting anchor.</summary>
	public void Close()
	{
		if (paper == null)
		{
			return;
		}

		StartAnimation(closedAnchoredPosition, closedSize);
	}

	private void StartAnimation(Vector2 targetPosition, Vector2 targetSize)
	{
		if (animationCoroutine != null)
		{
			StopCoroutine(animationCoroutine);
			animationCoroutine = null;
		}

		if (!isActiveAndEnabled)
		{
			paper.anchoredPosition = targetPosition;
			paper.sizeDelta = targetSize;
			return;
		}

		animationCoroutine = StartCoroutine(AnimatePaper(targetPosition, targetSize));
	}

	private IEnumerator AnimatePaper(Vector2 targetPosition, Vector2 targetSize)
	{
		Vector2 startPosition = paper.anchoredPosition;
		Vector2 startSize = paper.sizeDelta;
		float elapsed = 0f;

		while (elapsed < animationDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / animationDuration));
			paper.anchoredPosition = Vector2.LerpUnclamped(startPosition, targetPosition, progress);
			paper.sizeDelta = Vector2.LerpUnclamped(startSize, targetSize, progress);
			yield return null;
		}

		paper.anchoredPosition = targetPosition;
		paper.sizeDelta = targetSize;
		animationCoroutine = null;
	}
}
