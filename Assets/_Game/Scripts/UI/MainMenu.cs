using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _xpText;

    private void Start()
    {
        // Load data immediately when menu opens
        PlayerData data = SaveSystem.Load();

        if (_xpText != null)
        {
            _xpText.text = $"Total XP: {data.TotalXP}";
        }
    }
    public void PlayGame()
    {
        // Make sure your Level 1 scene is added to Build Settings!
        SceneManager.LoadScene("SampleScene_1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}