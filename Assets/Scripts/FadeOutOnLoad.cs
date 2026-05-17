using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeOutOnLoad : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1.2f;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 1f;
        canvasGroup.blocksRaycasts = true;
        while (t > 0f)
        {
            t -= Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}