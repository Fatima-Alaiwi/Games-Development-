using UnityEngine;

public class FruitBasket : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Pick up Fruit Basket";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Item Settings")]
    public string itemName = "Fruit";
    public Sprite itemIcon;
    public AudioClip collectSound;

  public void Interact()
{
    Debug.Log("FruitBasket Interact() called!"); // ✅ add this
    
    bool added = InventoryManager.instance.AddItem(itemName, itemIcon);
    Debug.Log("Item added: " + added); // ✅ add this

    if (added)
    {
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        if (QuestManager.Instance != null)
            QuestManager.Instance.UpdateProgress(itemName, 1);

        gameObject.SetActive(false);
    }
}
}