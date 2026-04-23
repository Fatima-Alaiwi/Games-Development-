using UnityEngine;

public class Farmer : MonoBehaviour, IInteractable
{
    [SerializeField] private string defaultText = "Talk to Farmer";

public string InteractionText
{
    get
    {
        if (hasGivenKey)
            return "Subarashii! Safe travels, stranger.";

        int count = GetItemCount();

        if (count >= requiredAmount)
            return "Give items to Farmer";

        if (count == 1)
            return "You are almost there...";

        return defaultText;
    }
}
//    public string InteractionText { get; set; } = "Talk to Farmer";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Key Item")]
    public Sprite keyIcon;
    public AudioClip collectSound;

    [Header("Dialogue")]
    public AudioClip greetingClip;   // 0 items - hint about fruit
    public AudioClip oneItemClip;    // has 1 item - ask for more
    public AudioClip thankYouClip;   // has 2 items - complete quest

    [Header("Quest Settings")]
    public Quest fruitQuest;
    public string requiredItemName = "Fruit";
    public int requiredAmount = 2;

    [Header("Animation")]
    private Animator animator;

    private bool hasGivenKey = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Get animator from this object or children
        animator = GetComponentInChildren<Animator>();
    }
public void Interact()
{
    if (animator != null)
        animator.SetBool("isTalking", true);

    int count = GetItemCount();
    Debug.Log("COUNT IS: " + count);

    if (!hasGivenKey && count >= requiredAmount)
    {
        // ✅ Has both items - give key
        hasGivenKey = true;
       // isInteractable = false;
        PlayClip(thankYouClip);
       // InteractionText = "Subarashii! Safe travels, stranger.";
        InventoryManager.instance.RemoveItem(requiredItemName, requiredAmount);
        bool added = InventoryManager.instance.AddItem("Key1", keyIcon);
        if (added && collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        Invoke(nameof(StopTalking), thankYouClip != null ? thankYouClip.length : 2f);
        return;
    }

    if (!hasGivenKey && count == 1)
    {
        // ✅ Has 1 item
        PlayClip(oneItemClip);
        //InteractionText = "You are almost there...";
        Invoke(nameof(StopTalking), oneItemClip != null ? oneItemClip.length : 2f);
        return;
    }

    if (!hasGivenKey && count == 0)
    {
        // ✅ No items - start quest
        PlayClip(greetingClip);
        //InteractionText = "Check back once you're done!";
        if (fruitQuest != null && QuestManager.Instance != null)
            QuestManager.Instance.AcceptQuest(fruitQuest);
        Invoke(nameof(StopTalking), greetingClip != null ? greetingClip.length : 2f);
        return;
    }
}

    void StopTalking()
    {
        if (animator != null)
            animator.SetBool("isTalking", false);
    }

    int GetItemCount()
    {
        if (InventoryManager.instance == null) return 0;
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