using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
// Raghad: attach this to Warp_Sphere inside the cell — Horror Mansion level only
// Player presses E to enter portal — no quest requirement
// Cell door already handles the key requirement
public class HorrorMansionPortalInteractable : MonoBehaviour, IInteractable
{
    [Header("Scene")]
    public string targetScene = "Demo_Dungeon_01";

    [Header("Sound")]
    public AudioClip teleportSound;
    // Raghad: drag Peter_12 audio file here
    public AudioClip peterPortalClip;
    private AudioSource audioSource;

    [Header("Interaction Text")]
    [SerializeField] private string _interactionText = "Press E to Enter Portal";
    public string InteractionText => _interactionText;
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

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
        GameObject canvasGO = new GameObject("HorrorMansionFadeCanvas");
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

        // Raghad: uses FadeOutOnLoad so fade works with next scene fade in
        FadeOutOnLoad fadeHelper = canvasGO.AddComponent<FadeOutOnLoad>();
        fadeHelper.canvasGroup = fadeCanvasGroup;
        fadeHelper.fadeDuration = fadeDuration;

        DontDestroyOnLoad(canvasGO);
    }

    public void Interact()
    {
        if (!isInteractable) return;
        if (_isTeleporting) return;

        _isTeleporting = true;
        isInteractable = false;

        StartCoroutine(TeleportSequence());
    }

    IEnumerator TeleportSequence()
    {
        // Raghad: play Peter's voice line — "I don't know where this leads. But anywhere is better than here."
        if (peterPortalClip != null && audioSource != null)
            audioSource.PlayOneShot(peterPortalClip);

        yield return new WaitForSeconds(1.5f);

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

        SceneManager.LoadScene(targetScene);
    }
}