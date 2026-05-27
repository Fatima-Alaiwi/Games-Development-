using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PortalController : MonoBehaviour, IInteractable
{
    [Header("Required Quests")]
    public Quest satelliteQuest;
    public Quest powerCellQuest;
    public Quest gunQuest;

    [Header("Next Level")]
    public string nextSceneName;
    public SavedValue.LevelId nextLevel = SavedValue.LevelId.Kingdom;

    [Header("Portal Components")]
    public GameObject probe;
    public GameObject warpSphere;
    public Animation spinningPiece;

    [Header("Sound")]
    public AudioClip voiceLineClip;
    public AudioClip teleportSound;

    [Header("IInteractable Settings")]
    [SerializeField] private string _readyText = "Press E to Enter Portal";
    [SerializeField] private string _missingGunText = "Pick up the gun first!";
    [SerializeField] private bool _isInteractable = false;
    [SerializeField] private Transform _labelAnchor;

    public string InteractionText => (gunQuest != null && gunQuest.isCompleted) ? _readyText : _missingGunText;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor != null ? _labelAnchor : transform;

    private AudioSource _audioSource;
    private CanvasGroup _fadeCanvasGroup;
    private readonly float _fadeDuration = 1.2f;
    private bool _isTeleporting = false;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        CreateFadeCanvas();
    }

    void Update()
    {
        bool technicalQuestsDone = (satelliteQuest != null && satelliteQuest.isCompleted) &&
                                   (powerCellQuest != null && powerCellQuest.isCompleted);

        if (technicalQuestsDone)
        {
            SetPortalState(true);
            isInteractable = (gunQuest != null && gunQuest.isCompleted);
        }
        else
        {
            SetPortalState(false);
            isInteractable = false;
        }
    }

    private void SetPortalState(bool active)
    {
        if (probe != null) probe.SetActive(active);

        if (warpSphere != null)
        {
            MeshRenderer mr = warpSphere.GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = active;
        }

        if (spinningPiece != null)
        {
            if (active) { if (!spinningPiece.isPlaying) spinningPiece.Play(); }
            else spinningPiece.Stop();
        }
    }

    public void Interact()
    {
        if (!isInteractable || _isTeleporting || string.IsNullOrEmpty(nextSceneName)) return;

        _isTeleporting = true;
        isInteractable = false;
        StartCoroutine(TeleportSequence());
    }

    IEnumerator TeleportSequence()
    {
        if (voiceLineClip != null && _audioSource != null)
            _audioSource.PlayOneShot(voiceLineClip);

        yield return new WaitForSeconds(1.5f);

        if (teleportSound != null && _audioSource != null)
            _audioSource.PlayOneShot(teleportSound);

        _fadeCanvasGroup.blocksRaycasts = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _fadeDuration;
            _fadeCanvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        SavedValue.SetCurrentLevel(nextLevel);
        SaveSystem.Save();

        if (LoadingScreenController.Instance != null)
            LoadingScreenController.Instance.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }

    private void CreateFadeCanvas()
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

        _fadeCanvasGroup = canvasGO.AddComponent<CanvasGroup>();
        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;

        FadeOutOnLoad fadeHelper = canvasGO.AddComponent<FadeOutOnLoad>();
        fadeHelper.canvasGroup = _fadeCanvasGroup;
        fadeHelper.fadeDuration = _fadeDuration;

        DontDestroyOnLoad(canvasGO);
    }
}
