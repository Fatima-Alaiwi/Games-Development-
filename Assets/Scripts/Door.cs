using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string InteractionText => "Press E to Open Door";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;
    public void Interact()
    {
        if (!isInteractable) return;

        // Perform the opening logic
        Debug.Log("Opening the mansion door...");
        transform.Rotate(0, 90, 0);

        // Disable further interaction so you can't "open" it again
        isInteractable = false;
    }
}