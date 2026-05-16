using UnityEngine;
using System.Collections;

public class PhoneInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to pick up the phone";
    public bool isInteractable { get; set; } = false;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Sounds")]
    public AudioSource phoneAudioSource;
    public AudioClip ringClip;
    public AudioClip screamClip;
    // Raghad: drag Peter_05 audio file here — plays 2 seconds after phone starts ringing
    public AudioClip peterPhoneClip;
    // Raghad: drag Peter_06 audio file here — plays 2 seconds after phone is picked up
    public AudioClip peterPanicClip;

    [Header("Spawner")]
    public EnemySpawner enemySpawner;

    [Header("Quest Settings")]
    public Quest answerPhoneQuest;

    private bool hasBeenPickedUp = false;

    public void StartRinging()
    {
        if (hasBeenPickedUp) return;

        if (phoneAudioSource != null && ringClip != null)
        {
            phoneAudioSource.clip = ringClip;
            phoneAudioSource.loop = true;
            phoneAudioSource.Play();
        }

        // Raghad: wait 2 seconds then play Peter's voice line
        StartCoroutine(PlayPeterLineAfterDelay(2f));

        isInteractable = true;
        Debug.Log("Phone is ringing!");
    }

    IEnumerator PlayPeterLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Only play if Peter hasn't picked up the phone yet
        if (!hasBeenPickedUp && peterPhoneClip != null && phoneAudioSource != null)
            phoneAudioSource.PlayOneShot(peterPhoneClip);
    }

    public void Interact()
    {
        if (hasBeenPickedUp) return;

        hasBeenPickedUp = true;
        isInteractable = false;
        InteractionText = "";

        // Raghad: complete phone quest
        if (answerPhoneQuest != null)
            QuestManager.Instance.UpdatedCompleteQuest(answerPhoneQuest);

        // Stop ringing AND Peter's voice line together
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
            enemySpawner.StartSpawning();
        else
            Debug.LogWarning("No EnemySpawner assigned to the phone!");

        // Raghad: wait 2 seconds then play Peter's panic line — "What, what are those things?! Stay back!"
        StartCoroutine(PlayPeterPanicAfterDelay(2f));

        Debug.Log("Phone picked up! Screaming started, enemies spawning!");
    }

    IEnumerator PlayPeterPanicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (peterPanicClip != null && phoneAudioSource != null)
            phoneAudioSource.PlayOneShot(peterPanicClip);
    }
}