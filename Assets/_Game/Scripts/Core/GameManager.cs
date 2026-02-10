using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // For restarting

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int _maxLives = 20;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private GameObject _gameOverPanel; // We will make this later

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
        // Time.timeScale = 0; // Pause game
        if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
    }

    private void UpdateUI()
    {
        if (_livesText != null) _livesText.text = $"Lives: {CurrentLives}";
    }
}