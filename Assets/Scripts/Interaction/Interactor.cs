using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    private Mouse mouse;
    public Camera mainCamera;

    void Awake()
    {
        mouse = Mouse.current;
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (mouse == null)
            return;

        // The Papers form or an active dialogue window owns input entirely - while either
        // is up, ignore world raycasts completely. Normal raycasting resumes the frame the
        // dialog box closes (DBoxControl.speaking goes false).
        if (PapersUI.IsOpen || DBoxControl.speaking)
            return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            // A click that landed on any UI element must never fall through to a world object.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            GameObject clickedObject = GetClickedObject();

            if (clickedObject != null && clickedObject.CompareTag("Clickable"))
            {
                if (clickedObject.TryGetComponent<Interactable>(out var interactable))
                {
                    interactable.OnInteract();
                }
                else
                {
                    Debug.Log("Clickable object has no Interactable component.");
                }
            }
        }
    }

    private GameObject GetClickedObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.collider.gameObject;

        return null;
    }
}
