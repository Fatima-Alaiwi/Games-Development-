using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;

    
    //will add the game level after
    public void StartGame()
    {
        //SceneManager.LoadScene("GameLevel");
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