using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using System.Linq;


public class CameraLook : MonoBehaviour
{
    [SerializeField] private float turnDegrees = 90f;
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float movementDuration = 0.35f;
    [SerializeField] private float rotationDuration = 0.35f;
    public Camera mainCamera;

    private Coroutine movementCoroutine;
    private Coroutine rotationCoroutine;

    void Start()
    {
        Application.targetFrameRate = 120;
        getCamTrans();
    }
    
    public void getCamTrans()
    {
        mainCamera = GetComponent<Camera>();
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
        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(SmoothRotate(degrees));
    }

    public void moveForward()
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(SmoothMoveForward());
    }

    // COROUTINES WOOOO
    private IEnumerator SmoothRotate(Vector3 degrees)
    {
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


}
