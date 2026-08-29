using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;


public class CameraLook : MonoBehaviour
{
    [SerializeField] private float turnDegrees = 90f;
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float movementDuration = 0.35f;
    [SerializeField] private float rotationDuration = 0.35f;
    [SerializeField] private float heightDuration = 0.3f;

    [SerializeField] private Button[] paneButtons;

    public Camera mainCamera;

    /// <summary>True while a 90 turn animation is in progress. Rotation input is ignored until it clears.</summary>
    public bool IsTurning { get; private set; }

    private Coroutine movementCoroutine;
    private Coroutine rotationCoroutine;
    private Coroutine heightCoroutine;

    void Start()
    {
        Application.targetFrameRate = 120;
        getCamTrans(); // Me too, camera

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null && gameManager.playerPaper != null)
        {
            SetCameraHeightFromPaper(gameManager.playerPaper, true);
        }
    }

    public void getCamTrans()
    {
        mainCamera = GetComponent<Camera>();
    }

    /// <summary>
    /// Drive the camera's world Y from the paper's height. <see cref="PlayerPaper.GetCameraHeight"/>
    /// already clamps to 120-230 cm. Smooth by default; pass instant for the initial placement.
    /// </summary>
    public void SetCameraHeightFromPaper(PlayerPaper paper, bool instant = false)
    {
        if (paper == null)
            return;

        if (mainCamera == null)
            getCamTrans();
        if (mainCamera == null)
            return;

        float targetY = paper.GetCameraHeight();

        if (heightCoroutine != null)
            StopCoroutine(heightCoroutine);

        if (instant || heightDuration <= 0f)
        {
            Vector3 snapped = mainCamera.transform.position;
            snapped.y = targetY;
            mainCamera.transform.position = snapped;
            heightCoroutine = null;
            return;
        }

        heightCoroutine = StartCoroutine(SmoothHeight(targetY));
    }

    private IEnumerator SmoothHeight(float targetY)
    {
        float startY = mainCamera.transform.position.y;
        float elapsed = 0f;

        while (elapsed < heightDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / heightDuration));

            Vector3 position = mainCamera.transform.position;
            position.y = Mathf.Lerp(startY, targetY, progress);
            mainCamera.transform.position = position;

            yield return null;
        }

        Vector3 final = mainCamera.transform.position;
        final.y = targetY;
        mainCamera.transform.position = final;
        heightCoroutine = null;
    }

    public void TurnLeft()
    {
        turnTo(Vector3.up * -turnDegrees);
    }

    public void TurnRight()
    {
        turnTo(Vector3.up * turnDegrees);
    }

    public void turnTo(Vector3 degrees)
    {
        if (IsTurning)
            return;

        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(SmoothRotate(degrees));
    }

    private void SetPaneButtonsInteractable(bool value)
    {
        if (paneButtons == null)
            return;

        foreach (Button button in paneButtons)
        {
            if (button != null)
                button.interactable = value;
        }
    }

    public void moveForward()
    {
        if (mainCamera == null)
            getCamTrans();

        if (mainCamera == null)
            return;

        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, 6f))
        {
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Clickable"))
            {
                movementCoroutine = StartCoroutine(SmoothMoveForwardBlocked());
                return;
            }
        }

        movementCoroutine = StartCoroutine(SmoothMoveForward());
    }

    // COROUTINES WOOOO
    private IEnumerator SmoothRotate(Vector3 degrees)
    {
        IsTurning = true;
        SetPaneButtonsInteractable(false);

        Quaternion startRotation = mainCamera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(degrees) * startRotation;
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / rotationDuration);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
            yield return null;
        }

        mainCamera.transform.rotation = targetRotation;
        rotationCoroutine = null;

        SetPaneButtonsInteractable(true);
        IsTurning = false;
    }

    private IEnumerator SmoothMoveForward()
    {
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = startPosition + mainCamera.transform.forward * moveDistance;
        float elapsed = 0f;

        while (elapsed < movementDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / movementDuration);
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        movementCoroutine = null;
    }

    private IEnumerator SmoothMoveForwardBlocked()
    {
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 bumpForward = mainCamera.transform.forward * Mathf.Min(1.5f, moveDistance * 0.25f);
        Vector3 bumpTarget = startPosition + bumpForward;
        float firstHalf = Mathf.Max(0.08f, movementDuration * 0.45f);
        float secondHalf = Mathf.Max(0.08f, movementDuration - firstHalf);
        float elapsed = 0f;

        while (elapsed < firstHalf)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / firstHalf);
            mainCamera.transform.position = Vector3.Lerp(startPosition, bumpTarget, progress);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < secondHalf)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / secondHalf);
            mainCamera.transform.position = Vector3.Lerp(bumpTarget, startPosition, progress);
            yield return null;
        }

        mainCamera.transform.position = startPosition;
        movementCoroutine = null;
    }

}
