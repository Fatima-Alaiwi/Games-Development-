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

    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Key Item")]
    public Sprite keyIcon;
    public AudioClip collectSound;

    [Header("Map Item")]
    public string mapItemName = "SamuraiMap";
    public Sprite mapIcon;
    public AudioClip mapVoiceLine;   // farmer tells player about the map
    public AudioClip mapPickupSound; // sound when map enters inventory
    private bool _hasGivenMap = false;

    [Header("Dialogue")]
    public AudioClip greetingClip;
    public AudioClip oneItemClip;
    public AudioClip thankYouClip;

    [Header("Quest Settings")]
    public Quest fruitQuest;
    public Quest gasQuest;
//    public Quest gasQuest;
    public string requiredItemName = "Fruit";
    public int requiredAmount = 2;

    [Header("Bell / Ambient Sound")]
    public AudioClip bellClip;
    public float bellTriggerRadius = 8f;

    [Header("Spawner")]
    public EnemySpawner villageSpawner;

    private Animator animator;
    private bool hasGivenKey = false;
    private AudioSource audioSource;
    private bool bellPlayed = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // farmerSphere is a sibling of Character_Village_Man,
        // so we go up to the root and search ALL children
        Transform root = transform.root;
        foreach (var anim in root.GetComponentsInChildren<Animator>(true))
        {
            if (anim.gameObject.name != "farmerSphere")
            {
                animator = anim;
                break;
            }
        }

        Debug.Log("Animator found: " + (animator != null ? animator.gameObject.name : "NULL"));

        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = bellTriggerRadius;
    }

    void OnTriggerEnter(Collider other)
    {
        if (bellPlayed) return;
        if (!other.CompareTag("Player")) return;

        if (bellClip != null)
            AudioSource.PlayClipAtPoint(bellClip, transform.position);

        bellPlayed = true;
    }

  public void Interact()
{
    if (!_hasGivenMap)
    {
        _hasGivenMap = true;
        InventoryManager.instance.AddItem(mapItemName, mapIcon);

        if (mapPickupSound != null)
            AudioSource.PlayClipAtPoint(mapPickupSound, transform.position);

        if (mapVoiceLine != null)
        {
            GameObject voiceObj = GameObject.Find("VoiceAudioSource");
            if (voiceObj != null)
                voiceObj.GetComponent<AudioSource>()?.PlayOneShot(mapVoiceLine);
        }
    }

    int count = GetItemCount();
    Debug.Log("COUNT IS: " + count);

    if (!hasGivenKey && count >= requiredAmount)
    {
        hasGivenKey = true;
        TriggerTalkingAnimation(thankYouClip);

        InventoryManager.instance.RemoveItem(requiredItemName, requiredAmount);
        bool added = InventoryManager.instance.AddItem("Key1", keyIcon);

        if (added && collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        if (QuestManager.Instance != null && fruitQuest != null)
            QuestManager.Instance.CompleteQuestPublic(fruitQuest);

        if (QuestManager.Instance != null && gasQuest != null)
            QuestManager.Instance.AcceptQuest(gasQuest);

        if (villageSpawner != null)
            villageSpawner.StartSpawning();

        return;
    }

    if (!hasGivenKey && count == 1)
    {
        TriggerTalkingAnimation(oneItemClip);
        return;
    }

    if (!hasGivenKey && count == 0)
    {
        TriggerTalkingAnimation(greetingClip);

        if (fruitQuest != null && QuestManager.Instance != null)
            QuestManager.Instance.AcceptQuest(fruitQuest);

        return;
    }
}

    void TriggerTalkingAnimation(AudioClip clip)
    {
        if (animator != null)
            animator.SetBool("isTalking", true);

        PlayClip(clip);

        float duration = clip != null ? clip.length : 2f;
        CancelInvoke(nameof(StopTalking));
        Invoke(nameof(StopTalking), duration);
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