using UnityEngine;

public class FollowSignTrigger : MonoBehaviour
{
    [Tooltip("Unique ID for this trigger — must match the GameObject name in the scene (e.g. enteringTheLevelSound).")]
    public string triggerId;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (string.IsNullOrEmpty(triggerId))
            triggerId = gameObject.name;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (AudioTriggerTracker.HasPlayed(triggerId)) return;

        audioSource.volume = 0.3f;
        audioSource.Play();
        AudioTriggerTracker.MarkPlayed(triggerId);
    }
}
