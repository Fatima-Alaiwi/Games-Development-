using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class FinalPortalMenu : MonoBehaviour, IInteractable
{
    [Header("Final Screen")]
    [Tooltip("The panel/image with the Main Menu button. Leave it inactive at the start.")]
    public GameObject finalScreenPanel;

    [Header("Main Menu")]
    [Tooltip("Exact name of the Main Menu scene in Build Settings.")]
    public string mainMenuScene = "MainMenu";

    [Header("Scene Visibility")]
    [Tooltip("When the final screen opens, hide everything in the current scene except the final canvas and UI event system.")]
    public bool hideSceneOnShow = true;

    [Header("Interaction")]
    [SerializeField] private string interactionText = "Press E to Enter Portal";
    [SerializeField] private Transform labelAnchor;
    [SerializeField] private bool interactable = true;

    public string InteractionText => interactionText;
    public bool isInteractable { get => interactable; set => interactable = value; }
    public Transform LabelAnchor => labelAnchor;

    private void Awake()
    {
        if (finalScreenPanel != null)
        {
            finalScreenPanel.SetActive(false);
        }
    }

    public void Interact()
    {
        if (!interactable)
        {
            return;
        }

        ShowFinalScreen();
    }

    public void ShowFinalScreen()
    {
        if (hideSceneOnShow)
        {
            HideSceneExceptFinalCanvas();
        }

        if (finalScreenPanel != null)
        {
            finalScreenPanel.SetActive(true);
        }

        PauseMenuManager.isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToMainMenu()
    {
        SaveSystem.DeleteSave();
        PauseMenuManager.isPaused = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);
    }

    private void HideSceneExceptFinalCanvas()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        GameObject finalScreenRoot = GetFinalScreenRoot();
        GameObject managerRoot = transform.root.gameObject;
        GameObject eventSystemRoot = GetEventSystemRoot();

        foreach (GameObject rootObject in rootObjects)
        {
            if (rootObject == finalScreenRoot ||
                rootObject == managerRoot ||
                rootObject == eventSystemRoot)
            {
                continue;
            }

            rootObject.SetActive(false);
        }
    }

    private GameObject GetFinalScreenRoot()
    {
        if (finalScreenPanel == null)
        {
            return null;
        }

        Canvas canvas = finalScreenPanel.GetComponentInParent<Canvas>(true);

        if (canvas != null)
        {
            return canvas.transform.root.gameObject;
        }

        return finalScreenPanel.transform.root.gameObject;
    }

    private GameObject GetEventSystemRoot()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);

        if (eventSystem == null)
        {
            return null;
        }

        return eventSystem.transform.root.gameObject;
    }
}
