using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SamuraiPortalController : MonoBehaviour, IInteractable
{
    [Header("Required Quest")]
    public Quest bambooQuest;

    [Header("Scene")]
    public string targetScene;
    [Tooltip("The level this portal leads to — saved so the menu shows the correct level on Continue.")]
    public SavedValue.LevelId nextLevel;

    [Header("Sound")]
    public AudioClip teleportSound;
    private AudioSource audioSource;

    [Header("Fade")]
    public float audioDelay = 1.5f;
    public float fadeDuration = 1.2f;
    private CanvasGroup _fadeCanvasGroup;

    [Header("Interaction Text")]
    [SerializeField] private string _questText = "Complete your quests first...";
    [SerializeField] private string _unlockedText = "Press E to Enter Portal";

    // IInteractable
    public string InteractionText => GetInteractionText();
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    private bool _isTeleporting = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        CreateFadeCanvas();
    }

    void CreateFadeCanvas()
    {
        GameObject canvasGO = new GameObject("SamuraiFadeCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject panelGO = new GameObject("FadePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image img = panelGO.AddComponent<Image>();
        img.color = Color.black;
        RectTransform rect = panelGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        _fadeCanvasGroup = canvasGO.AddComponent<CanvasGroup>();
        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;

        FadeOutOnLoad fadeHelper = canvasGO.AddComponent<FadeOutOnLoad>();
        fadeHelper.canvasGroup = _fadeCanvasGroup;
        fadeHelper.fadeDuration = fadeDuration;

        DontDestroyOnLoad(canvasGO);
    }

    void Update()
    {
        if (!_isTeleporting)
            isInteractable = true;
    }

    string GetInteractionText()
    {
        if (!IsQuestComplete()) return _questText;
        return _unlockedText;
    }

    bool IsQuestComplete()
    {
        if (bambooQuest == null) return false;
        return QuestManager.Instance.IsQuestCompleted(bambooQuest.questName);
    }

    public void Interact()
    {
        if (!isInteractable) return;
        if (!IsQuestComplete()) return;

        _isTeleporting = true;
        isInteractable = false;
        StartCoroutine(TeleportSequence());
    }

    IEnumerator TeleportSequence()
    {
        if (teleportSound != null)
            audioSource.PlayOneShot(teleportSound);

        yield return new WaitForSeconds(audioDelay);

        _fadeCanvasGroup.blocksRaycasts = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            _fadeCanvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }
        _fadeCanvasGroup.alpha = 1f;

        SavedValue.SetCurrentLevel(nextLevel);
        SaveSystem.Save();

        if (LoadingScreenController.Instance != null)
            LoadingScreenController.Instance.LoadScene(targetScene);
        else
            SceneManager.LoadScene(targetScene);
    }

}