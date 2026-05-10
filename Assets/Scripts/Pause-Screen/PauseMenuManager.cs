using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static bool isPaused = false;

    [Header("Main Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject saveConfirmPanel;
    public GameObject instructionsPanel;
    public GameObject restartConfirmPanel;
    public GameObject quitConfirmationPanel;


    void Start()
    {
        
        HideAllPanels();
        // 2. Reset the game state
        isPaused = false;
        Time.timeScale = 1f;

        // 3. Lock the cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void OpenPausePanel()
    {
        HideAllPanels();
        pauseMenuPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
    }

    public void OpenSaveConfirm()
    {
        HideAllPanels();
        saveConfirmPanel.SetActive(true);
    }

    public void OpenInstructions()
    {
        HideAllPanels();
        instructionsPanel.SetActive(true);
    }

    public void OpenRestartConfirm()
    {
        HideAllPanels();
        restartConfirmPanel.SetActive(true);
    }

    private void HideAllPanels()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        saveConfirmPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        restartConfirmPanel.SetActive(false);
        quitConfirmationPanel.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        HideAllPanels();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        OpenPausePanel(); 
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ConfirmRestart()
    {
        Time.timeScale = 1f;
        OpenPausePanel();
    }

    public void ConfirmSave()
    {
        Debug.Log("Game Saved to Slot!");
        OpenPausePanel();
    }

    public void ExitToMain()
    {
        Application.Quit();
    }

    // Opens the confirm window
public void OpenQuitConfirm()
{
    quitConfirmationPanel.SetActive(true);
}

// Closes the confirm window
public void CloseQuitConfirm()
{
    quitConfirmationPanel.SetActive(false);
}

// Actually quits the game
public void QuitGame()
{
    Debug.Log("Quitting game...");
    
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
}
}