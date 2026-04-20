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
            audioSource.Play();
            hasPlayed = true; // so it only plays once
        }
    }
}