using UnityEngine;
using TMPro; // If you want to change button text color later

public class TimeController : MonoBehaviour
{
    private float _currentSpeed = 1f;

    private void Update()
    {
        // Keyboard Shortcuts for testing (1, 2, 3 keys)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetSpeed(1f);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetSpeed(2f);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetSpeed(4f); // 3 key = 4x speed
    }

    public void SetSpeed(float speed)
    {
        _currentSpeed = speed;
        Time.timeScale = _currentSpeed;

        Debug.Log($"Game Speed: {speed}x");
    }

    // Helper to pause/unpause without losing the speed setting
    public void TogglePause()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0; // Pause
        }
        else
        {
            Time.timeScale = _currentSpeed; // Resume at previous speed
        }
    }
}