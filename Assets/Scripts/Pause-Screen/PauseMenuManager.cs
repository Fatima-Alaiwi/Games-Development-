using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
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
        EnsureEventSystemExists();
        HideAllPanels();
        isPaused = false;
        Time.timeScale = 1f;

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
        SetPanelActive(pauseMenuPanel, true);
    }

    public void OpenSettings()
    {
        HideAllPanels();
        SetPanelActive(settingsPanel, true);
    }

    public void OpenSaveConfirm()
    {
        HideAllPanels();
        SetPanelActive(saveConfirmPanel, true);
    }

    public void OpenInstructions()
    {
        HideAllPanels();
        SetPanelActive(instructionsPanel, true);
    }

    public void OpenRestartConfirm()
    {
        HideAllPanels();
        SetPanelActive(restartConfirmPanel, true);
    }

    private void HideAllPanels()
    {
        SetPanelActive(pauseMenuPanel, false);
        SetPanelActive(settingsPanel, false);
        SetPanelActive(saveConfirmPanel, false);
        SetPanelActive(instructionsPanel, false);
        SetPanelActive(restartConfirmPanel, false);
        SetPanelActive(quitConfirmationPanel, false);
    }

    public void Resume()
    {
        isPaused = false;
        HideAllPanels();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        OpenPausePanel(); 
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
        SetPanelActive(quitConfirmationPanel, true);
    }

    // Closes the confirm window
    public void CloseQuitConfirm()
    {
        SetPanelActive(quitConfirmationPanel, false);
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

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }

    private void EnsureEventSystemExists()
    {

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();

        InputSystemUIInputModule inputModule = eventSystemObject.AddComponent<InputSystemUIInputModule>();
        inputModule.AssignDefaultActions();
    }
}
