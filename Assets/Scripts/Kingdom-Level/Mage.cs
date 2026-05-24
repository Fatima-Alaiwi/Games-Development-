using UnityEngine;
using System.Collections;

public class Mage : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to talk to the Mage";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Dialogue Clips")]
    public AudioClip greetingClip;
    public AudioClip waitClip;
    public AudioClip rewardClip;
    public AudioClip completedClip;

    [Header("Peter Voice Lines")]
    public AudioClip peterClip;

    [Header("Quest Requirements")]
    public Quest magesTreasureQuest;
    public Quest returnHomeQuest;

    [Header("Item Requirements")]
    public string item1Name = "Gold";
    public string item2Name = "Key";

    [Header("Potion Reward")]
    public GameObject potionPrefab;
    public Transform potionSpawnPoint;

    private bool hasHandedOverTreasure = false;
    private bool playerIsNearby = false;
    private AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerIsNearby && !hasHandedOverTreasure && Input.GetKeyDown(KeyCode.Q))
        {
            TryGiveTreasure();
        }
    }

    public void Interact()
    {
        if (!isInteractable) return;

        SetTalkingAnimation(true);

        if (!hasHandedOverTreasure)
        {
            PlayClip(greetingClip);
            Debug.Log("Mage: Bring me Gold and a Key, then press Q to hand them over!");
            StartCoroutine(StopTalkingAfterAudio(greetingClip));
        }
        else
        {
            AudioClip played = completedClip != null ? completedClip : rewardClip;
            PlayClip(played);
            StartCoroutine(StopTalkingAfterAudio(played));
        }
    }

    private void TryGiveTreasure()
    {
        bool hasItem1 = InventoryManager.instance.HasItem(item1Name);
        bool hasItem2 = InventoryManager.instance.HasItem(item2Name);

        if (hasItem1 && hasItem2)
        {
            hasHandedOverTreasure = true;
            SetTalkingAnimation(true);

            // Remove both items
            InventoryManager.instance.RemoveItem(item1Name, 1);
            InventoryManager.instance.RemoveItem(item2Name, 1);
            Debug.Log($"Handed over {item1Name} and {item2Name}!");

            // Complete quest
            if (magesTreasureQuest != null)
                QuestManager.Instance.UpdatedCompleteQuest(magesTreasureQuest);

            // Play mage reward audio
            PlayClip(rewardClip);

            // Start next quest
            if (returnHomeQuest != null)
                QuestManager.Instance.AcceptQuest(returnHomeQuest);

            // Spawn potion
            if (potionPrefab != null)
            {
                Vector3 spawnPos = potionSpawnPoint != null
                    ? potionSpawnPoint.position
                    : transform.position - Vector3.up * 0.5f;
                Instantiate(potionPrefab, spawnPos, Quaternion.identity);
                Debug.Log("Potion spawned!");
            }

            // Play Peter's audio reaction line right after the Mage finishes speaking
            float rewardLength = rewardClip != null ? rewardClip.length : 0f;
            if (peterClip != null)
            {
                StartCoroutine(PlayPeterLineAfterDelay(rewardLength));
            }
            else
            {
                StartCoroutine(StopTalkingAfterAudio(rewardClip));
            }
        }
        else
        {
            // Missing one or both items
            SetTalkingAnimation(true);
            PlayClip(waitClip);
            Debug.Log($"Mage: I need both {item1Name} and {item2Name}.");
            StartCoroutine(StopTalkingAfterAudio(waitClip));
        }
    }

    IEnumerator PlayPeterLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay + 0.2f);
        if (peterClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(peterClip);
        }
        yield return new WaitForSeconds(peterClip != null ? peterClip.length : 2f);
        SetTalkingAnimation(false);
    }

    #region Trigger Proximity
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerIsNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            SetTalkingAnimation(false);
        }
    }
    #endregion

    #region Helpers
    IEnumerator StopTalkingAfterAudio(AudioClip clip)
    {
        float wait = clip != null ? clip.length : 2f;
        yield return new WaitForSeconds(wait);
        SetTalkingAnimation(false);
    }

    void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void SetTalkingAnimation(bool talking)
    {
        if (animator != null)
            animator.SetBool("isTalking", talking);
    }
    #endregion
}