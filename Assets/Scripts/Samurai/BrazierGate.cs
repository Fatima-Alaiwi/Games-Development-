using UnityEngine;

public class BrazierGate : MonoBehaviour
{
    [Header("Settings")]
    public int braziersRequired = 2;
    public BrazierDoor door; // ✅ changed from Door to BrazierDoor
    public AudioClip gateOpenSound;

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

        if (braziersLit >= braziersRequired)
        {
            if (gateOpenSound != null && audioSource != null)
                audioSource.PlayOneShot(gateOpenSound);

            if (door != null)
                door.OpenFromBrazier();
        }
    }
}