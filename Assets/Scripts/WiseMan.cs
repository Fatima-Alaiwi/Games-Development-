using UnityEngine;

public class WiseMan : MonoBehaviour
{
    [Header("Key Item")]
    public Sprite keyIcon;
    public AudioClip collectSound;

    bool hasGivenKey = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasGivenKey)
        {
            hasGivenKey = true;

            bool added = InventoryManager.instance.AddItem("DungeonKey", keyIcon);

            if (added && collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
    }
}