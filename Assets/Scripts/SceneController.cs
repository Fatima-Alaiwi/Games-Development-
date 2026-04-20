using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The main menu panel with Start/Settings/Quit buttons")]
    public GameObject mainPath; 
    
    [Tooltip("The settings panel you created with sliders")]
    public GameObject settingsPath;


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }

    public void OpenSettings()
    {
        if (mainPath != null && settingsPath != null)
        {
            mainPath.SetActive(false);
            settingsPath.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (mainPath != null && settingsPath != null)
        {
            settingsPath.SetActive(false);
            mainPath.SetActive(true);
        }
    }
}