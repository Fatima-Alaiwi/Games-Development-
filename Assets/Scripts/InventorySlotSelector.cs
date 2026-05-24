using UnityEngine;
using UnityEngine.UI;

public class InventorySlotSelector : MonoBehaviour
{
    public static InventorySlotSelector instance;

    [Header("Slots")]
    public InventorySlot[] slots;

    [Header("Selection Highlight")]
    public Color selectedColor = new Color(1f, 1f, 0f, 0.4f); // yellow highlight
    public Color normalColor = new Color(1f, 1f, 1f, 0f);     // transparent

    [Header("Bomb Throwing")]
    public GameObject bombPrefab;       // drag your bomb model prefab here
    public Transform throwOrigin;       // leave empty = uses Camera
    public AudioClip throwSound;

    [Header("Medicine")]
    public AudioClip medicineSound;

    private int selectedIndex = 0;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        HandleScrollSelection();
        HandleUseItem();
        UpdateHighlight();
    }

    void HandleScrollSelection()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = slots.Length - 1;
        }
        else if (scroll < 0f)
        {
            selectedIndex++;
            if (selectedIndex >= slots.Length) selectedIndex = 0;
        }
    }

    void HandleUseItem()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        if (selectedIndex >= InventoryManager.instance.items.Count) return;

        InventoryManager.InventoryItem selectedItem = InventoryManager.instance.items[selectedIndex];

        // Handle bomb separately
        if (selectedItem.itemName == "DungeonBomb")
        {
            ThrowBomb();
            InventoryManager.instance.RemoveItem("DungeonBomb", 1);
            return;
        }

        int healAmount = 0;

        switch (selectedItem.itemName)
        {
            case "Zink":        healAmount = 2; break;
            case "Pills":       healAmount = 3; break;
            case "Vitamins":    healAmount = 4; break;
            case "FirstAidKit": healAmount = 8; break;
            case "Potion": healAmount = 4; break;
            

            default:
                Debug.Log(selectedItem.itemName + " cannot be used.");
                return;
        }

        // Find the player Actor
        Actor playerActor = null;
        Actor[] allActors = FindObjectsByType<Actor>(FindObjectsSortMode.None);
        foreach (Actor a in allActors)
        {
            if (a.isPlayer) { playerActor = a; break; }
        }

        if (playerActor == null) return;

        // Block use if health is already full
        if (playerActor.currentHealth >= playerActor.maxHealth)
        {
            Debug.Log("Health is already full!");
            return;
        }

        playerActor.Heal(healAmount);

        // Play medicine sound
        if (medicineSound != null)
        {
            AudioSource playerAudio = playerActor.GetComponent<AudioSource>();
            if (playerAudio != null) playerAudio.PlayOneShot(medicineSound);
        }

        InventoryManager.instance.RemoveItem(selectedItem.itemName, 1);
        Debug.Log("Used " + selectedItem.itemName + " and healed " + healAmount + " HP.");
    }

    void ThrowBomb()
    {
        if (bombPrefab == null) { Debug.LogError("Bomb prefab not assigned!"); return; }

        Camera cam = Camera.main;
        Vector3 spawnPos = cam.transform.position + cam.transform.forward * 1.5f;
        Vector3 throwDir = cam.transform.forward;

        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        Collider col = bomb.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
        }

        BombProjectile projectile = bomb.GetComponent<BombProjectile>();
        if (projectile == null)
            projectile = bomb.AddComponent<BombProjectile>();

        projectile.Launch(throwDir);
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();
            if (slotImage != null)
                slotImage.color = (i == selectedIndex) ? selectedColor : normalColor;
        }
    }
}