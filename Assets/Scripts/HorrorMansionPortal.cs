using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
// Raghad: attach this to Warp_Sphere inside the cell portal
// Creates the fade canvas automatically in code — no need to create Canvas in Unity
// Make sure Is Trigger is checked on the Sphere Collider of Warp_Sphere
public class HorrorMansionPortal : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName = "Demo_Dungeon_01";

    [Header("Fade")]
    public float fadeDuration = 1.2f;

    [Header("Sound")]
    // Raghad: drag Peter_12 audio file here — plays when entering the portal
    public AudioClip peterPortalClip;
    private AudioSource audioSource;

    private bool hasTriggered = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        // Play Peter's voice line — "I don't know where this leads. But anywhere is better than here."
        if (peterPortalClip != null && audioSource != null)
            audioSource.PlayOneShot(peterPortalClip);

        StartCoroutine(FadeOutAndLoad());
    }

    IEnumerator FadeOutAndLoad()
    {
        // Wait for Peter's line to feel natural then start fading
        yield return new WaitForSeconds(1.5f);

        // Create fade canvas automatically in code — same as teammate's SceneFadeIn
        GameObject canvasGO = new GameObject("FadeOutCanvas");
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

        CanvasGroup cg = canvasGO.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = true;

        // Fade to black
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            cg.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        cg.alpha = 1f;

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }
}