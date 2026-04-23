using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Pick up Key";
    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        QuestManager.Instance.UpdateProgress("key_1", 1);
        gameObject.SetActive(false); // Key disappears after pickup
    }
}