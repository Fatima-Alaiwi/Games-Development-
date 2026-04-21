using UnityEngine;

public class WiseMan : MonoBehaviour
{
    [Header("Key Item")]
    public Sprite keyIcon;
    public AudioClip collectSound;

    [Header("Dialogue")]
    public AudioClip greetingClip;
    public AudioClip thankYouClip;

    [Header("Gold Requirement")]
    public int requiredGoldCount = 3;

    private bool hasGivenKey = false;
    private bool hasPlayedGreeting = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        int goldCount = GetGoldCount();

        if (!hasGivenKey && goldCount >= requiredGoldCount)
        {
            hasGivenKey = true;
            PlayClip(thankYouClip);

            InventoryManager.instance.RemoveItem("DungeonGold", 3);

            bool added = InventoryManager.instance.AddItem("DungeonKey", keyIcon);
            if (added && collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        else if (!hasPlayedGreeting && !hasGivenKey)
        {
            hasPlayedGreeting = true;
            PlayClip(greetingClip);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            hasPlayedGreeting = false;
    }

    int GetGoldCount()
    {
        foreach (var item in InventoryManager.instance.items)
        {
            if (item.itemName == "DungeonGold")
                return item.count;
        }
        return 0;
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
}