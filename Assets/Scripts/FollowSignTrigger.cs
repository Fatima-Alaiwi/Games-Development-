using UnityEngine;

public class FollowSignTrigger : MonoBehaviour
{
    AudioSource audioSource;
    bool hasPlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player") && !hasPlayed)
    {
        audioSource.volume = 0.3f; // change this value (0 = silent, 1 = full)
        audioSource.Play();
        hasPlayed = true;
    }
}
}