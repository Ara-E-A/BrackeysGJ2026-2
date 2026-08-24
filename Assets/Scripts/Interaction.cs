using UnityEngine;

public class Interaction : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // LMB
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    Debug.Log("Cube clicked!");
                }
            }
        }
    }
}