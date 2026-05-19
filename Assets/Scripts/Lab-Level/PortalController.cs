using UnityEngine;

public class PortalController : MonoBehaviour, IInteractable
{
    [Header("Required Quests")]
    public Quest satelliteQuest;
    public Quest powerCellQuest;
    public Quest gunQuest;

    [Header("Portal Components")]
    public GameObject probe;
    public GameObject warpSphere;
    public Animation spinningPiece;

    [Header("IInteractable Settings")]
    [SerializeField] private string _readyText = "Press E to Enter Portal";
    [SerializeField] private string _missingGunText = "Pick up the gun first!";
    [SerializeField] private bool _isInteractable = false;
    [SerializeField] private Transform _labelAnchor;

    // Interface Properties
    public string InteractionText => (gunQuest != null && gunQuest.isCompleted) ? _readyText : _missingGunText;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;

    void Update()
    {
        // Directly check the isCompleted boolean on the ScriptableObjects
        bool technicalQuestsDone = (satelliteQuest != null && satelliteQuest.isCompleted) && 
                                    (powerCellQuest != null && powerCellQuest.isCompleted);

        if (technicalQuestsDone)
        {
            SetPortalState(true);
            
            // The portal only becomes interactable once the gun quest is finished
            isInteractable = (gunQuest != null && gunQuest.isCompleted);
        }
        else
        {
            SetPortalState(false);
            isInteractable = false;
        }
    }

    private void SetPortalState(bool active)
    {
        // Toggle the visual pieces
        if (probe != null) probe.SetActive(active);
        if (warpSphere != null) warpSphere.SetActive(active);

        // Handle the spinning animation
        if (spinningPiece != null)
        {
            if (active)
            {
                if (!spinningPiece.isPlaying) spinningPiece.Play();
            }
            else
            {
                spinningPiece.Stop();
            }
        }
    }

    public void Interact()
    {
        if (isInteractable)
        {
            Debug.Log("<color=purple>PORTAL:</color> Conditions met. Transitioning level...");
        }
    }
}