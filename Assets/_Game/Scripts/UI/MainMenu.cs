using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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