using UnityEngine;

public class SatellitePanel : MonoBehaviour, IInteractable
{
    public SatelliteController satelliteScript;

    [field: SerializeField] public string InteractionText { get; set; } = "Align Satellite";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    public void Interact()
    {
        if (satelliteScript != null)
        {
            satelliteScript.StartControlling();
        }
    }
}