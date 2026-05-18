using UnityEngine;

public class BrazierGate : MonoBehaviour
{
    [Header("Settings")]
    public int braziersRequired = 2;
    public AudioClip gateOpenSound; // keep for when door actually opens

    private int braziersLit = 0;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public bool BothLit()
    {
        return braziersLit >= braziersRequired;
    }

    public void BrazierLit()
    {
        braziersLit++;
        Debug.Log("Braziers lit: " + braziersLit + "/" + braziersRequired);
        // No auto-open anymore — player must go interact with door using key
    }

    public void PlayOpenSound()
    {
        if (gateOpenSound != null && audioSource != null)
            audioSource.PlayOneShot(gateOpenSound);
    }
}