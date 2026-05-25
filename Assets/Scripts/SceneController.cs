using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;

    [Header("Continue Button")]
    [Tooltip("Assign the Continue button so it hides when no save file exists.")]
    public Button continueButton;

    [Header("First Level")]
    [Tooltip("Scene name to load when starting a brand-new game.")]
    public string firstLevelScene = "Demo_01";

    void Start()
    {
        if (continueButton != null)
            continueButton.interactable = SaveSystem.HasSave();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelScene);
    }

    public void ContinueGame()
    {
        if (!SaveSystem.HasSave()) return;
        SaveSystem.ContinueGame();
    }

    public void QuitGame()
    {
        Debug.Log("Exiting Time Voyager...");
        Application.Quit();
    }

    public void OpenPanel(GameObject panelToOpen)
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        panelToOpen.SetActive(true);
    }
    public void BackToMain()
    {
        OpenPanel(mainPanel);
    }
}