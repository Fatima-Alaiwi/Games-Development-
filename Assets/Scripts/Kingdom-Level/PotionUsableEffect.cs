using UnityEngine;
using System.Collections;

public class PotionUsableEffect : MonoBehaviour
{
    [Header("Potion Configuration")]
    public string potionItemName = "Potion"; // Ensure this matches your PickupObject item name string exactly

    [Header("Potion Screen Effect UI")]
    public Canvas effectCanvas;
    public UnityEngine.UI.Image blueOverlay; // A full-screen Image child on that canvas, blue + low alpha

    private bool isEffectPlaying = false;

    void Start()
    {
        // Hide overlay on start
        if (blueOverlay != null)
            blueOverlay.color = new Color(0f, 0.3f, 1f, 0f);
    }

    void Update()
    {
        // Listen globally for player hitting Q to drink the potion if they have it
        if (Input.GetKeyDown(KeyCode.Q) && !isEffectPlaying)
        {
            // Verify with inventory manager if player has already picked it up
            if (InventoryManager.instance != null && InventoryManager.instance.HasItem(potionItemName))
            {
                ConsumePotion();
            }
        }
    }

    private void ConsumePotion()
    {
        isEffectPlaying = true;

        // Remove 1 potion from the inventory slot setup
        InventoryManager.instance.RemoveItem(potionItemName, 1);
        Debug.Log("Potion consumed! Triggering dizzy overlay effect.");

        // Fire screen visual sequence
        StartCoroutine(PlayDizzyEffect());
    }

    IEnumerator PlayDizzyEffect()
    {
        if (blueOverlay == null) yield break;

        float fadeDuration = 1f;
        float holdDuration = 2.5f;
        float maxAlpha = 0.55f;

        // Fade in blue overlay
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            blueOverlay.color = new Color(0f, 0.3f, 1f, Mathf.Lerp(0f, maxAlpha, t));

            // Dizzy pulse wave modifications
            float pulse = Mathf.Sin(t * Mathf.PI * 6f) * 0.08f;
            Color c = blueOverlay.color;
            blueOverlay.color = new Color(c.r, c.g, c.b, Mathf.Clamp(c.a + pulse, 0f, 1f));

            yield return null;
        }

        // Hold the active dizzy state
        float held = 0f;
        while (held < holdDuration)
        {
            held += Time.deltaTime;
            float pulse = Mathf.Sin(held * Mathf.PI * 4f) * 0.1f;
            blueOverlay.color = new Color(0f, 0.3f, 1f, Mathf.Clamp(maxAlpha + pulse, 0f, 1f));
            yield return null;
        }

        // Fade out back to clear screen
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            blueOverlay.color = new Color(0f, 0.3f, 1f, Mathf.Lerp(maxAlpha, 0f, t));
            yield return null;
        }

        blueOverlay.color = new Color(0f, 0.3f, 1f, 0f);
        isEffectPlaying = false;
    }
}