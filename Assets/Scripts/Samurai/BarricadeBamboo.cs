using UnityEngine;
using System.Collections;

public class BarricadeBamboo : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip fallSound;

    private bool isChopped = false;
    private BambooBarricadeManager manager;

    public void SetManager(BambooBarricadeManager m)
    {
        manager = m;
    }

    public void GetChopped()
    {
        if (isChopped) return;
        isChopped = true;
        StartCoroutine(FallOver());
    }

    IEnumerator FallOver()
    {
        if (fallSound != null)
            AudioSource.PlayClipAtPoint(fallSound, transform.position);

        float elapsed = 0f;
        float duration = 0.6f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, 90f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            yield return null;
        }

        transform.rotation = endRot;

        // Tell manager this bamboo is cut
        if (manager != null)
            manager.OnBambooCut();

        // Hide after falling
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}