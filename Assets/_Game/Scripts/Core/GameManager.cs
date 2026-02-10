using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Required for reloading the scene

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int _maxLives = 5; // Reduced for testing

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private GameObject _gameOverPanel; // <--- Drag your Panel here!

    public int CurrentLives { get; private set; }
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        CurrentLives = _maxLives;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (IsGameOver) return;

        CurrentLives -= damage;
        UpdateUI();

        if (CurrentLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        IsGameOver = true;
        Debug.Log("GAME OVER!");

        // Show the panel
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
        }
    }

    // Call this from the Restart Button
    public void RestartGame()
    {
        // Reloads the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateUI()
    {
        if (_livesText != null) _livesText.text = $"Lives: {CurrentLives}";
    }
}