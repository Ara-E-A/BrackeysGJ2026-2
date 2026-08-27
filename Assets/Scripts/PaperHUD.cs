using System.Collections;
using TMPro;
using UnityEngine;

public class PaperHUD : MonoBehaviour
{
	[SerializeField] private RectTransform paper;
	[SerializeField] private TextMeshProUGUI informationText;
	[SerializeField] private float expandedScale = 2.5f;
	[SerializeField] private float animationDuration = 0.35f;

	private Vector3 originalPosition;
	private Vector2 originalSize;
	private Coroutine animationCoroutine;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			HideImportantInformation();
		}
	}

	private void Awake()
	{
		if (paper == null)
		{
			paper = GetComponent<RectTransform>();
		}

		if (informationText == null)
		{
			informationText = GetComponentInChildren<TextMeshProUGUI>(true);
		}

		if (paper != null)
		{
			originalPosition = paper.position;
			originalSize = paper.sizeDelta;
		}

		SetInformationVisible(false);
	}

	public void ShowImportantInformation(string information)
	{
		if (paper == null || informationText == null)
		{
			Debug.LogError("PaperHUD needs a paper RectTransform and a TMP information text.");
			return;
		}

		informationText.text = information;
		SetInformationVisible(false);
		StartAnimation(GetExpandedPosition(), originalSize * expandedScale, true);
	}

	public void HideImportantInformation()
	{
		if (paper == null)
		{
			return;
		}

		SetInformationVisible(false);
		StartAnimation(originalPosition, originalSize, false);
	}

	private Vector3 GetExpandedPosition()
	{
		RectTransform parent = paper.parent as RectTransform;
		if (parent == null)
		{
			return paper.position;
		}

		return parent.TransformPoint(parent.rect.center);
	}

	private void StartAnimation(Vector3 targetPosition, Vector2 targetSize, bool showTextWhenComplete)
	{
		if (animationCoroutine != null)
		{
			StopCoroutine(animationCoroutine);
		}

		animationCoroutine = StartCoroutine(AnimatePaper(targetPosition, targetSize, showTextWhenComplete));
	}

	private IEnumerator AnimatePaper(Vector3 targetPosition, Vector2 targetSize, bool showTextWhenComplete)
	{
		Vector3 startPosition = paper.position;
		Vector2 startSize = paper.sizeDelta;
		float elapsed = 0f;

		while (elapsed < animationDuration)
		{
			elapsed += Time.unscaledDeltaTime;
			float progress = Mathf.Clamp01(elapsed / animationDuration);
			progress = Mathf.SmoothStep(0f, 1f, progress);
			paper.position = Vector3.LerpUnclamped(startPosition, targetPosition, progress);
			paper.sizeDelta = Vector2.LerpUnclamped(startSize, targetSize, progress);
			yield return null;
		}

		paper.position = targetPosition;
		paper.sizeDelta = targetSize;
		SetInformationVisible(showTextWhenComplete);
		animationCoroutine = null;
	}

	private void SetInformationVisible(bool visible)
	{
		if (informationText != null)
		{
			informationText.enabled = visible;
		}
	}
}
