using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SamuraiPortalController : MonoBehaviour, IInteractable
{
    [Header("Required Quest")]
    public Quest bambooQuest;

    [Header("Scene")]
    public string targetScene;

    [Header("Sound")]
    public AudioClip teleportSound;
    private AudioSource audioSource;

    [Header("Interaction Text")]
    [SerializeField] private string _lockedText = "Strike the gong first...";
    [SerializeField] private string _questText = "Complete your quests first...";
    [SerializeField] private string _unlockedText = "Press E to Enter Portal";

    // IInteractable
    public string InteractionText => GetInteractionText();
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    private bool _gongStruck = false;
    private bool _isTeleporting = false;
    private CanvasGroup fadeCanvasGroup;
    private float fadeDuration = 1.2f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        CreateFadeCanvas();
    }

    void Update()
    {
        if (!_isTeleporting)
            isInteractable = true;
    }

    string GetInteractionText()
    {
        if (!_gongStruck) return _lockedText;
        if (!IsQuestComplete()) return _questText;
        return _unlockedText;
    }

    bool IsQuestComplete()
    {
        if (bambooQuest == null) return false;
        return QuestManager.Instance.IsQuestCompleted(bambooQuest.questName);
    }

    // Called by GongInteractable
    public void ActivatePortal()
    {
        _gongStruck = true;
        Debug.Log("<color=green>SAMURAI PORTAL:</color> Gong struck!");
    }

    public void Interact()
    {
        if (!isInteractable) return;
        if (!_gongStruck) return;
        if (!IsQuestComplete()) return;

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

        SaveSystem.Save();
        SceneManager.LoadScene(targetScene);
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
}