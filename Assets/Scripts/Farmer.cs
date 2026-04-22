using UnityEngine;

public class Farmer : MonoBehaviour
{
    [Header("Key Item")]
    public Sprite keyIcon;
    public AudioClip collectSound;

    [Header("Dialogue")]
    public AudioClip greetingClip;    // 0 items - hint about purple fruit
    public AudioClip oneItemClip;     // has 1 item - ask for more
    public AudioClip thankYouClip;    // has 2 items - complete quest

    [Header("Quest Settings")]
    public string requiredItemName = "PurpleFruit";
    public int requiredAmount = 2;

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

        int count = GetItemCount();

        if (!hasGivenKey && count >= requiredAmount)
        {
            // Has both items - complete quest
            hasGivenKey = true;
            PlayClip(thankYouClip);

            // Remove fruits from inventory
            InventoryManager.instance.RemoveItem(requiredItemName, requiredAmount);

            // Give key
            bool added = InventoryManager.instance.AddItem("Key1", keyIcon);
            if (added && collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        else if (count == 1 && !hasGivenKey)
        {
            // Has only 1 item
            PlayClip(oneItemClip);
        }
        else if (!hasPlayedGreeting && !hasGivenKey)
        {
            // First encounter - no items
            hasPlayedGreeting = true;
            PlayClip(greetingClip);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        hasPlayedGreeting = false;
    }

    int GetItemCount()
    {
        foreach (var item in InventoryManager.instance.items)
        {
            if (item.itemName == requiredItemName)
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