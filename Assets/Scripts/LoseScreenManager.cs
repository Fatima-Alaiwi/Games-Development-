using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreenManager : MonoBehaviour
{
    public static LoseScreenManager Instance { get; private set; }
    public static bool IsLoseScreenOpen { get; private set; }

    [Header("Lose Screen")]
    [Tooltip("Your lose screen panel from the prefab. Leave it inactive at the start.")]
    public GameObject loseScreenPanel;

    [Header("Main Menu")]
    [Tooltip("Exact name of the Main Menu scene in Build Settings.")]
    public string mainMenuScene = "MainMenu";

    private void Awake()
    {
        Instance = this;
        IsLoseScreenOpen = false;

        if (loseScreenPanel != null)
        {
            loseScreenPanel.SetActive(false);
        }
    }

    public static void Show()
    {
        if (Instance == null)
        {
            Instance = FindFirstObjectByType<LoseScreenManager>(FindObjectsInactive.Include);

            if (Instance == null)
            {
                Debug.LogWarning("LoseScreenManager is missing from the scene.");
                return;
            }
        }

        Instance.ShowLoseScreen();
    }

    public void ShowLoseScreen()
    {
        IsLoseScreenOpen = true;
        ClosePauseMenus();

        if (loseScreenPanel != null)
        {
            loseScreenPanel.SetActive(true);
        }

        PauseMenuManager.isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TryAgain()
    {
        IsLoseScreenOpen = false;
        PauseMenuManager.isPaused = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        IsLoseScreenOpen = false;
        PauseMenuManager.isPaused = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);
    }

    private void ClosePauseMenus()
    {
        PauseMenuManager[] pauseMenus = FindObjectsByType<PauseMenuManager>(FindObjectsSortMode.None);

        foreach (PauseMenuManager pauseMenu in pauseMenus)
        {
            pauseMenu.CloseAllPausePanels();
        }
    }
}
