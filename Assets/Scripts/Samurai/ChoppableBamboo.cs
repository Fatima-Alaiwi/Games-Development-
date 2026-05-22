using UnityEngine;
using System.Collections;

public class ChoppableBamboo : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Hit with melee to chop bamboo";

    public bool isInteractable { get; set; } = true;

    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Item Settings")]
    public string itemName = "Bamboo";
    public Sprite itemIcon;
    public AudioClip collectSound;

    [Header("Bamboo Sound")]
    public AudioClip fallSound;

    [Header("Quest Settings")]
    public Quest bambooQuest;

    private bool isChopped = false;
    private bool isCollectable = false;

    public bool IsCollectable => isCollectable;

    public void GetChopped()
    {
        if (isChopped) return;

        Debug.Log("Bamboo was chopped!");

        isChopped = true;
        InteractionText = "Bamboo falling...";
        StartCoroutine(FallOver());
    }

    IEnumerator FallOver()
    {
        float elapsed = 0f;
        float duration = 0.6f;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, 90f);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0f, 0.35f, 0f);

        // Play sound immediately when it starts falling
        if (fallSound != null)
            AudioSource.PlayClipAtPoint(fallSound, transform.position);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        transform.rotation = endRot;
        transform.position = endPos;

        // Keep collider enabled so E can detect it
        isCollectable = true;
        isInteractable = true;
        InteractionText = "Press E to collect Bamboo";

        Debug.Log("Bamboo is now collectable!");
    }

    public void Interact()
    {
        if (!isCollectable)
        {
            Debug.Log("Bamboo must be chopped with melee first.");
            return;
        }

        if (InventoryManager.instance == null)
        {
            Debug.LogError("InventoryManager instance is missing!");
            return;
        }

        bool added = InventoryManager.instance.AddItem(itemName, itemIcon);

        if (added)
        {
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            if (QuestManager.Instance != null && bambooQuest != null)
            {
                if (!QuestManager.Instance.activeQuests.Contains(bambooQuest) &&
                    !QuestManager.Instance.completedQuests.Contains(bambooQuest))
                {
                    QuestManager.Instance.AcceptQuest(bambooQuest);
                }

                QuestManager.Instance.UpdateProgress(itemName, 1);
            }

            gameObject.SetActive(false);
        }
    }
}