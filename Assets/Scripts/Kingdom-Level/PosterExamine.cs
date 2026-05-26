using UnityEngine;
using UnityEngine.UI;

public class PosterExamine : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to View";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Poster Settings")]
    public Sprite posterImage;
    public AudioClip playerVoiceClip;

    [Header("UI References")]
    public GameObject examineUI;
    public Image posterDisplay;

    private bool isExamining = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        examineUI.SetActive(false);
    }

    void Update()
    {
        if (isExamining && Input.GetKeyDown(KeyCode.E))
            ClosePoster();
    }

    public void Interact()
    {
        if (!isExamining)
            OpenPoster();
    }

    void OpenPoster()
    {
        isExamining = true;
        examineUI.SetActive(true);
        posterDisplay.sprite = posterImage;

        if (playerVoiceClip != null && audioSource != null)
            audioSource.PlayOneShot(playerVoiceClip);
    }

    void ClosePoster()
    {
        isExamining = false;
        examineUI.SetActive(false);
    }
}