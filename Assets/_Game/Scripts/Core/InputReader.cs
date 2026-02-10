using UnityEngine;
using UnityEngine.InputSystem; // Make sure Input System package is installed
using UnityEngine.EventSystems;

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
        // 1. UI BLOCKER CHECK
        // If the mouse is over a UI element (Button, Panel), STOP here.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 2. Normal Logic
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = -_mainCamera.transform.position.z;
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);

        GridNode node = LevelGridManager.Instance.GetNodeAtWorldPosition(worldPos);

        if (node != null)
        {
            LevelGridManager.Instance.TryPlaceTower(node.GridPosition);
        }
    }
}