using UnityEngine;

public interface IInteractable
{

    string InteractionText { get; }
    bool isInteractable {get; set;}
    Transform LabelAnchor { get; }
    void Interact();
}