using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string InteractionText => "Press E to Open Door";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;
    public void Interact()
    {
        transform.Rotate(0, 90, 0);
        isInteractable = false;  
    }
}