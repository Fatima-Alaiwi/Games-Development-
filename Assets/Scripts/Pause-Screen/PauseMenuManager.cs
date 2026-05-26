using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static bool isPaused = false;
    private const int PauseCanvasSortingOrder = 10000;

    [Header("Main Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject saveConfirmPanel;
    public GameObject instructionsPanel;
    public GameObject restartConfirmPanel;
    public GameObject quitConfirmationPanel;

    [Header("HUD")]
    [Tooltip("Drag the Canvas that contains the health bar here.")]
    public GameObject hudCanvas;
    [Tooltip("Drag the Canvas (2) that contains the inventory slots here.")]
    public GameObject inventoryCanvas;
    [Tooltip("Drag the Quest HUD panel here.")]
    public GameObject questPanel;
    [Tooltip("Drag the Ammo display panel here.")]
    public GameObject ammoPanel;

    private Canvas pauseCanvas;
    private bool originalOverrideSorting;
    private int originalSortingOrder;

    void Start()
    {
        EnsureEventSystemExists();
        CachePauseCanvasSorting();

        HideAllPanels();

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Temporary debug
        GameObject examine = GameObject.Find("ExamineUI");
        if (examine != null)
            Debug.Log("ExamineUI active: " + examine.activeSelf.ToString());
        else
            Debug.Log("ExamineUI is NULL");
    }

    void Update()
    {
        if (LoseScreenManager.IsLoseScreenOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
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

    public void CloseAllPausePanels()
    {
        HideAllPanels();
        RestorePauseCanvasSorting();
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
        RestorePauseCanvasSorting();

        if (hudCanvas != null) hudCanvas.SetActive(true);
        if (inventoryCanvas != null) inventoryCanvas.SetActive(true);
        if (questPanel != null) questPanel.SetActive(true);
        if (ammoPanel != null) ammoPanel.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        isPaused = true;

        Time.timeScale = 0f;

        if (hudCanvas != null) hudCanvas.SetActive(false);
        if (inventoryCanvas != null) inventoryCanvas.SetActive(false);
        if (questPanel != null) questPanel.SetActive(false);
        if (ammoPanel != null) ammoPanel.SetActive(false);

        BringPauseCanvasToFront();
        OpenPausePanel();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ConfirmRestart()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ConfirmSave()
    {
        SaveSystem.Save();
        OpenPausePanel();
    }

    [Header("Main Menu")]
    [Tooltip("Exact name of your Main Menu scene in Build Settings.")]
    public string mainMenuScene = "MainMenu";

    public void ExitToMain()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(mainMenuScene);
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
        Time.timeScale = 1f;
        isPaused = false;

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

    private void CachePauseCanvasSorting()
    {
        pauseCanvas = GetComponent<Canvas>();

        if (pauseCanvas == null)
            return;

        originalOverrideSorting = pauseCanvas.overrideSorting;
        originalSortingOrder = pauseCanvas.sortingOrder;
    }

    private void BringPauseCanvasToFront()
    {
        if (pauseCanvas == null)
            return;

        pauseCanvas.overrideSorting = true;
        pauseCanvas.sortingOrder = PauseCanvasSortingOrder;
    }

    private void RestorePauseCanvasSorting()
    {
        if (pauseCanvas == null)
            return;

        pauseCanvas.overrideSorting = originalOverrideSorting;
        pauseCanvas.sortingOrder = originalSortingOrder;
    }

    private void EnsureEventSystemExists()
    {
        EventSystem existingEventSystem = FindFirstObjectByType<EventSystem>();

        if (existingEventSystem != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");

        eventSystemObject.AddComponent<EventSystem>();

        InputSystemUIInputModule inputModule =
            eventSystemObject.AddComponent<InputSystemUIInputModule>();

        inputModule.AssignDefaultActions();
    }
}
