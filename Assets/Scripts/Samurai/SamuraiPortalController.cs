using UnityEngine;
using UnityEngine.SceneManagement;

public class SamuraiPortalController : MonoBehaviour, IInteractable
{
    [Header("Required Quests")]
    public Quest bambooQuest;

    [Header("Portal Components")]
    public GameObject probe;
    public GameObject warpSphere;
    public Animation spinningPiece;

    [Header("IInteractable Settings")]
    [SerializeField] private string _readyText = "Press E to Enter Portal";
    [SerializeField] private string _notReadyText = "Strike the gong first...";
    [SerializeField] private bool _isInteractable = false;
    [SerializeField] private Transform _labelAnchor;

    [Header("Next Scene")]
    [SerializeField] private string _nextSceneName;

    // Interface Properties
    public string InteractionText => _isInteractable ? _readyText : _notReadyText;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor;

    private bool _gongStruck = false;

    void Start()
    {
        SetPortalState(false);
    }

    void Update()
    {
        if (_gongStruck)
        {
            SetPortalState(true);
            isInteractable = bambooQuest != null && bambooQuest.isCompleted;
        }
        else
        {
            SetPortalState(false);
            isInteractable = false;
        }
    }

    // Called by GongInteractable when gong is struck
    public void ActivatePortal()
    {
        _gongStruck = true;
        Debug.Log("<color=green>SAMURAI PORTAL:</color> Gong struck. Portal activated!");
    }

    private void SetPortalState(bool active)
    {
        if (probe != null) probe.SetActive(active);
        if (warpSphere != null) warpSphere.SetActive(active);

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
            Debug.Log("<color=green>SAMURAI PORTAL:</color> Conditions met. Transitioning level...");
            if (!string.IsNullOrEmpty(_nextSceneName))
                SceneManager.LoadScene(_nextSceneName);
        }
    }
}