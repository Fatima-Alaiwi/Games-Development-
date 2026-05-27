using UnityEngine;

public class BrazierGate : MonoBehaviour
{
    [Header("Settings")]
    public int braziersRequired = 2;
    public AudioClip gateOpenSound; // keep for when door actually opens

    private int braziersLit = 0;
    private AudioSource audioSource;
    [Header("Spawner")]
    public EnemySpawner gateSpawner;

    [Header("Quest")]
    public Quest killQuestGate;
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

        if (BothLit())
        {
            if (gateSpawner != null)
                gateSpawner.StartSpawning();

            if (killQuestGate != null && QuestManager.Instance != null)
                QuestManager.Instance.AcceptQuest(killQuestGate);
        }
    }

    public void PlayOpenSound()
    {
        if (gateOpenSound != null && audioSource != null)
            audioSource.PlayOneShot(gateOpenSound);
    }
}