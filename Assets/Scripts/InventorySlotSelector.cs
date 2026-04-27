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
 
        // Make sure selected slot has an item
        if (selectedIndex >= InventoryManager.instance.items.Count) return;
 
        InventoryManager.InventoryItem selectedItem = InventoryManager.instance.items[selectedIndex];
 
        // Only bombs are usable
        if (selectedItem.itemName == "DungeonBomb")
        {
            ThrowBomb();
            InventoryManager.instance.RemoveItem("DungeonBomb", 1);
        }
        else
        {
            Debug.Log(selectedItem.itemName + " cannot be used directly.");
        }
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