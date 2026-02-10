using UnityEngine;
using UnityEngine.InputSystem; // Make sure Input System package is installed

public class InputReader : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // Simple polling for now (fast to implement). 
        // Can be converted to Events later if needed.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();

        // Ensure the Z position creates a ray that hits the world (usually -10 to 0)
        mousePos.z = -_mainCamera.transform.position.z;

        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);

        // Get the node at this position
        GridNode node = LevelGridManager.Instance.GetNodeAtWorldPosition(worldPos);

        if (node != null)
        {
            // Try to build!
            LevelGridManager.Instance.TryPlaceTower(node.GridPosition);
        }
    }
}