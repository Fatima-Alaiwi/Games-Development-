using UnityEngine;

public class CollectiblePotion : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Press E to Pick Up Potion";
    public string InteractionText => _interactionText;

    public bool isInteractable { get; set; } = true;

    [SerializeField] private Transform _labelAnchor;
    public Transform LabelAnchor => _labelAnchor != null ? _labelAnchor : transform;

    [Header("Quest Connection")]
    [Tooltip("Drag the Talk To Merchant quest asset here so the potion spawner can trigger the next phase")]
    public Quest talkToMerchantQuest;

    public void Interact()
    {
        if (!isInteractable) return;
        isInteractable = false;

        Debug.Log("Peter picked up the healing potion bottle!");

        if (QuestManager.Instance != null)
        {
            // 1. Tell system that 1 item matching the "HealingPotion" goal name was found.
            // This safely increments the amount, handles clamping, and auto-completes the quest!
            QuestManager.Instance.UpdateProgress("HealingPotion", 1);

            // 2. Since Peter now has the bottle, activate the next tracking quest link (Talk to Merchant)
            if (talkToMerchantQuest != null)
            {
                QuestManager.Instance.AcceptQuest(talkToMerchantQuest);
            }
        }

        // 3. Remove this specific spawned clone from the map
        Destroy(gameObject);
    }
}