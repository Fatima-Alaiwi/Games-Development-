using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


//raghad try :)
public class LockedDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionText = "Press E to Enter Code";
    public string InteractionText => _interactionText;
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

    private bool isOpen = false;
    private bool panelOpen = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Hide panel at start
        if (codePanel != null)
            codePanel.SetActive(false);
    }

    public void Interact()
    {
        if (!isInteractable || isOpen) return;

        if (!panelOpen)
        {
            // Show code panel
            panelOpen = true;
            codePanel.SetActive(true);

            // Unlock cursor so player can type
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
            // Correct code!
            feedbackText.text = "Access Granted!";
            feedbackText.color = Color.green;

            // Close panel
            codePanel.SetActive(false);
            panelOpen = false;

            // Lock cursor again
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Open door
            isOpen = true;
            isInteractable = false;

            if (audioSource != null && openingDoorClip != null)
                audioSource.PlayOneShot(openingDoorClip);

            StartCoroutine(OpenDoor());
        }
        else
        {
            // Wrong code!
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