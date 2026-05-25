using UnityEngine;

/// <summary>
/// Attach to the Map collectible GameObject in your scene.
/// When the player presses E while looking at it (via your IInteractable system),
/// it adds "Map" to InventoryManager and calls MapUI.OnMapPickedUp().
///
/// Implements IInteractable so it works with your existing raycast interaction system.
/// </summary>
public class MapPickupItem : MonoBehaviour, IInteractable
{
    [Header("References")]
    public MapUI mapUI;                          // drag the MapPanel GameObject here

    [Header("Interaction Prompt")]
    public string promptText = "Pick up Map [E]";
    public Transform labelAnchor;                // shown on HUD Canvas as per your IInteractable system

    [Header("Inventory")]
    [Tooltip("Must match MapUI.mapItemName exactly.")]
    public string mapItemName = "Map";
    public Sprite mapSprite;                     // UI sprite for InventoryManager

    [Header("Visuals")]
    public float bobAmplitude = 0.1f;
    public float bobSpeed     = 1.5f;

    private Vector3 _startPos;
    private bool    _pickedUp = false;

    void Start()  => _startPos = transform.position;

    void Update()
    {
        if (_pickedUp) return;
        // Gentle floating bob so the item catches the player's eye
        transform.position = _startPos + Vector3.up *
            (Mathf.Sin(Time.time * bobSpeed) * bobAmplitude);
    }

    // ── IInteractable ────────────────────────────────────────────────────
    // REPLACE with these (matching your IInteractable interface):
    public string InteractionText => promptText;
    public bool isInteractable { get => !_pickedUp; set { } }
    public Transform LabelAnchor => labelAnchor != null ? labelAnchor : transform;

    public void Interact()
    {
        if (_pickedUp) return;
        _pickedUp = true;

        InventoryManager.instance?.AddItem(mapItemName, mapSprite);

        if (mapUI != null)
            mapUI.OnMapPickedUp();
        else
        {
            MapUI found = FindAnyObjectByType<MapUI>(FindObjectsInactive.Include);
            found?.OnMapPickedUp();
        }

        AudioSource audio = GetComponent<AudioSource>();
        audio?.Play();

        Destroy(gameObject, 0.3f);
    }

}
