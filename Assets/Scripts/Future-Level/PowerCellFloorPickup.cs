using UnityEngine;

public class PowerCellFloorPickup : MonoBehaviour, IInteractable
{
    [Header("Item Data")]
    public string itemName = "PowerCell";
    public Sprite itemIcon;
    public AudioClip collectSound;

    [Header("Interaction Settings")]
    [SerializeField] private string _interactionText = "Press [E] to Pick up Power Cell";
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;

    public string InteractionText => _interactionText;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;

    public void Interact()
    {
        if (!isInteractable) return;

        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            if (collectSound != null) 
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            if (UIManager.Instance != null)
                UIManager.Instance.HideHoverText(); 

            Destroy(gameObject);
        }
    }
}