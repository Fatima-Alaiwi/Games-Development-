using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PortalInteractable : MonoBehaviour, IInteractable
{
    [Header("Dragon Requirement")]
    public Quest dragonQuest;

    [Header("Scene")]
    public string targetScene = "Demo";
    [Tooltip("The level this portal leads to — saved so the menu shows the correct level on Continue.")]
    public SavedValue.LevelId nextLevel = SavedValue.LevelId.Lab;

    [Header("Sound")]
    public AudioClip teleportSound;
    private AudioSource audioSource;

    [Header("Interaction Text")]
    [SerializeField] private string _lockedText = "Kill the Dragon First";
    [SerializeField] private string _unlockedText = "Press E to Enter Portal";

    // IInteractable
    public string InteractionText => IsDragonDead() ? _unlockedText : _lockedText;
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    // Fade
    private CanvasGroup fadeCanvasGroup;
    private float fadeDuration = 1.2f;
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
        GameObject canvasGO = new GameObject("PortalFadeCanvas");
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

        fadeCanvasGroup = canvasGO.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;

        FadeOutOnLoad fadeHelper = canvasGO.AddComponent<FadeOutOnLoad>();
        fadeHelper.canvasGroup = fadeCanvasGroup;
        fadeHelper.fadeDuration = fadeDuration;

        DontDestroyOnLoad(canvasGO);
    }

    bool IsDragonDead()
    {
        if (dragonQuest == null) return false;
        return QuestManager.Instance.IsQuestCompleted(dragonQuest.questName);
    }

    void Update()
    {
        if (!_isTeleporting)
            isInteractable = true;
    }

    public void Interact()
    {
        if (!isInteractable) return;

        if (!IsDragonDead())
            return;

        _isTeleporting = true;
        isInteractable = false;
        StartCoroutine(TeleportSequence());
    }

    IEnumerator TeleportSequence()
    {
        if (teleportSound != null)
            audioSource.PlayOneShot(teleportSound);

        float t = 0f;
        fadeCanvasGroup.blocksRaycasts = true;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        SavedValue.SetCurrentLevel(nextLevel);
        SaveSystem.Save();
        SceneManager.LoadScene(targetScene);
    }
}