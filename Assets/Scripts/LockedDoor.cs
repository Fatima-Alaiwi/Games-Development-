using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Press E to Enter Code";
    [SerializeField] private string _lockedInteractionText = "Get the code from the Magician first!";
    public string InteractionText => GetInteractionText();
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Quest Requirement")]
    public Quest requiredQuest;   // drag BottleQuest here
    public Quest questToComplete; // drag FindLibraryQuest here
    public Quest portalKeyQuest;  // drag FindPortalKeyQuest here

    [Header("Code Settings")]
    public string correctCode = "888";

    [Header("Opening Settings")]
    [Tooltip("Absolute Y rotation (world space) when the door is fully open. E.g. if closed = 90, open = 200.")]
    public float openYRotation = 200f;
    public float openSpeed = 2f;

    [Header("Sound")]
    public AudioClip openingDoorClip;
    private AudioSource audioSource;

    [Header("UI")]
    public GameObject codePanel;
    public TMP_InputField codeInputField;
    public TextMeshProUGUI feedbackText;

    private bool isOpen = false;
    private bool panelOpen = false;
    private SaveableDoor _saveable;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        TryGetComponent(out _saveable);

        if (codePanel != null)
            codePanel.SetActive(false);
    }

    private string GetInteractionText()
    {
        if (isOpen)
            return "";

        if (requiredQuest != null && !QuestManager.Instance.IsQuestComplete(requiredQuest))
            return _lockedInteractionText;

        return _interactionText;
    }

    public void Interact()
    {
        if (!isInteractable || isOpen) return;

        if (requiredQuest != null && !QuestManager.Instance.IsQuestComplete(requiredQuest))
        {
            UIManager.Instance.ShowHoverText(_lockedInteractionText, transform.position);
            return;
        }

        if (!panelOpen)
        {
            panelOpen = true;
            codePanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (feedbackText != null)
                feedbackText.text = "";
        }
    }

    public void SubmitCode()
    {
        string enteredCode = codeInputField.text.Trim();

        if (enteredCode == correctCode)
        {
            feedbackText.text = "Access Granted!";
            feedbackText.color = Color.green;

            codePanel.SetActive(false);
            panelOpen = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isOpen = true;
            isInteractable = false;

            if (_saveable != null) _saveable.MarkOpened();

            if (questToComplete != null)
                QuestManager.Instance.UpdatedCompleteQuest(questToComplete);

            if (portalKeyQuest != null)
                QuestManager.Instance.AcceptQuest(portalKeyQuest);

            if (audioSource != null && openingDoorClip != null)
                audioSource.PlayOneShot(openingDoorClip);

            StartCoroutine(OpenDoor());
        }
        else
        {
            feedbackText.text = "Wrong Code. Try Again.";
            feedbackText.color = Color.red;
            codeInputField.text = "";
        }
    }

    public void ClosePanel()
    {
        codePanel.SetActive(false);
        panelOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SnapOpen()
    {
        isOpen = true;
        isInteractable = false;
        if (codePanel != null) codePanel.SetActive(false);
        Vector3 e = transform.eulerAngles;
        transform.eulerAngles = new Vector3(e.x, openYRotation, e.z);
    }

    IEnumerator OpenDoor()
    {
        Quaternion startRotation = transform.rotation;
        Vector3 e = transform.eulerAngles;
        Quaternion endRotation = Quaternion.Euler(e.x, openYRotation, e.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
    }
}