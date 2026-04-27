using UnityEngine;

public class PhoneInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to pick up the phone";
    public bool isInteractable { get; set; } = false; // Starts FALSE - only interactable after ringing
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Sounds")]
    public AudioSource phoneAudioSource;
    public AudioClip ringClip;       // Drag PhoneRing here
    public AudioClip screamClip;     // Drag Screaming here

    [Header("Spawner")]
    public EnemySpawner enemySpawner; // Drag your EnemySpawner object here

    [Header("Quest Settings")]
    public Quest answerPhoneQuest; // Raghad: drag AnswerPhoneQuest here

    private bool hasBeenPickedUp = false;  // Makes sure this happens only ONCE

    // Called by BasementTrigger when Peter enters the basement
    public void StartRinging()
    {
        if (hasBeenPickedUp) return; // Already picked up, do nothing

        if (phoneAudioSource != null && ringClip != null)
        {
            phoneAudioSource.clip = ringClip;
            phoneAudioSource.loop = true;   // Keep ringing until picked up
            phoneAudioSource.Play();
        }

        isInteractable = true; // Now Peter can pick it up
        Debug.Log("Phone is ringing!");
    }

    // Called when Peter presses E on the phone
    public void Interact()
    {
        if (hasBeenPickedUp) return;

        hasBeenPickedUp = true;
        isInteractable = false;
        InteractionText = ""; // Remove interaction text

        // Raghad: complete phone quest so HUD switches to enemy counter
        if (answerPhoneQuest != null)
            QuestManager.Instance.UpdatedCompleteQuest(answerPhoneQuest);

        // Stop ringing
        if (phoneAudioSource != null)
        {
            phoneAudioSource.Stop();
            phoneAudioSource.loop = false;
        }

        // Play screaming
        if (phoneAudioSource != null && screamClip != null)
        {
            phoneAudioSource.clip = screamClip;
            phoneAudioSource.loop = false;
            phoneAudioSource.Play();
        }

        // Activate the enemy spawner
        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning();
        }
        else
        {
            Debug.LogWarning("No EnemySpawner assigned to the phone!");
        }

        Debug.Log("Phone picked up! Screaming started, enemies spawning!");
    }
}