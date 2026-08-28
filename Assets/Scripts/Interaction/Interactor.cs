using UnityEngine;
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

        if (PapersUI.IsOpen)
            return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
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
