using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DungeonLockedDoor : MonoBehaviour, IInteractable
{
    [field: SerializeField]
    public string InteractionText { get; set; } = "Press E to Enter Code";
    public bool isInteractable { get; set; } = true;
    public Transform labelAnchor;
    public Transform LabelAnchor => labelAnchor;

    [Header("Code Settings")]
    public string correctCode = "888";

    [Header("Opening Settings")]
    public float openAngle = -90f;
    public float openSpeed = 2f;

    [Header("Sound")]
    public AudioClip openingDoorClip;
    private AudioSource audioSource;

    [Header("UI")]
    public GameObject codePanel;
    public TMP_InputField codeInputField;
    public TextMeshProUGUI feedbackText;

    [Header("Quest")]
    public Quest doorQuest;

    private bool isOpen = false;
    private bool panelOpen = false;
    private bool questStarted = false;
    public AudioClip doorLockedClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (codePanel != null)
            codePanel.SetActive(false);

        if (doorQuest != null)
            doorQuest.ResetQuest();
    }

    public void Interact()
{
    if (!isInteractable || isOpen) return;

    if (!questStarted)
    {
        questStarted = true;
        if (doorQuest != null)
            QuestManager.Instance.AcceptQuest(doorQuest);
    }

    if (!panelOpen)
    {
        panelOpen = true;
        codePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (feedbackText != null)
            feedbackText.text = "";

        // Play "door is locked" voice line
        if (audioSource != null && doorLockedClip != null)
            audioSource.PlayOneShot(doorLockedClip);
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

            GetComponent<SaveableDoor>()?.MarkOpened();

            if (audioSource != null && openingDoorClip != null)
                audioSource.PlayOneShot(openingDoorClip);

            if (doorQuest != null)
                QuestManager.Instance.CompleteQuestPublic(doorQuest);

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
        transform.rotation = transform.rotation * Quaternion.Euler(0, openAngle, 0);
    }

    IEnumerator OpenDoor()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, openAngle, 0);

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