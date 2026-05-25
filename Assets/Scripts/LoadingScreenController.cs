using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    public static LoadingScreenController Instance;

    [Header("Loading Image")]
    [Tooltip("Drag your loading screen image/sprite here.")]
    public Image loadingImage;

    [Header("UI")]
    public CanvasGroup canvasGroup;

    [Header("Timing")]
    [Tooltip("Minimum seconds the loading screen stays visible.")]
    public float minimumDisplayTime = 2f;
    public float fadeSpeed = 0.4f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSequence(sceneName));
    }

    IEnumerator LoadSequence(string sceneName)
    {
        // Fade in
        canvasGroup.blocksRaycasts = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeSpeed;
            canvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Load scene in background
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        if (op == null)
        {
            Debug.LogError($"[LoadingScreen] Scene '{sceneName}' could not be loaded. Make sure it is added to Build Settings (File → Build Settings).");
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            yield break;
        }
        op.allowSceneActivation = false;

        float elapsed = 0f;
        while (elapsed < minimumDisplayTime || op.progress < 0.9f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;

        // Fade out
        t = 1f;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime / fadeSpeed;
            canvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
