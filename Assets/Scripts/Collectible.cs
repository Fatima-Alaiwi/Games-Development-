using UnityEngine;

// Attach to any pickup. Give it a unique collectibleId in the Inspector.
// The save system uses this ID to hide the object when loading a save.
public class Collectible : MonoBehaviour
{
    [Tooltip("Must be unique within the scene, e.g. 'Kingdom_GoldIngot_1'.")]
    public string collectibleId;

    public void MarkCollected()
    {
        if (string.IsNullOrEmpty(collectibleId))
        {
            Debug.LogWarning($"[Collectible] '{gameObject.name}' has no collectibleId set — won't be tracked.");
            return;
        }
        CollectibleTracker.MarkCollected(collectibleId);
    }
}
