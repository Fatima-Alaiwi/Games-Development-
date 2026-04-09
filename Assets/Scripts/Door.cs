using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    // 1. This variable will now show up in the Unity Inspector
    [SerializeField] private string _interactionText = "Press E to Open Door";

    // 2. The interface now pulls the value from that variable
    public string InteractionText => _interactionText;

    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        if (!isInteractable) return;

        transform.Rotate(0, 90, 0);
        isInteractable = false;  
    }
}