using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ElevatorInterior : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private bool _isInteractable = true;
    [SerializeField] private Transform _labelAnchor;
    [SerializeField] private string _interactionText = "Press [E] to Use Elevator";

    [Header("Teleport")]
    public Transform upstairsTeleportPoint;
    public Transform playerTransform;
    public float fadeDuration = 1.2f;
    public float blackScreenHoldTime = 0.4f;
    public AudioClip elevatorUseSound;

    private AudioSource _audioSource;
    private CanvasGroup _fadeCanvasGroup;
    private bool _isTeleporting;

    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public Transform LabelAnchor => _labelAnchor != null ? _labelAnchor : transform;
    public string InteractionText => _interactionText;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        CreateFadeCanvas();
    }

    public void Interact()
    {
        if (!_isInteractable || _isTeleporting) return;
        if (upstairsTeleportPoint == null)
        {
            Debug.LogWarning("ElevatorInterior: Upstairs teleport point is not assigned.");
            return;
        }

        StartCoroutine(UseElevatorSequence());
    }

    private IEnumerator UseElevatorSequence()
    {
        _isTeleporting = true;
        _isInteractable = false;

        if (elevatorUseSound != null)
        {
            _audioSource.PlayOneShot(elevatorUseSound);
        }

        yield return FadeTo(1f);
        yield return new WaitForSeconds(blackScreenHoldTime);

        TeleportPlayer();

        yield return FadeTo(0f);

        _isTeleporting = false;
        _isInteractable = true;
    }

    private void TeleportPlayer()
    {
        Transform targetPlayer = playerTransform;
        if (targetPlayer == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetPlayer = player.transform;
            }
        }

        if (targetPlayer == null)
        {
            Debug.LogWarning("ElevatorInterior: Could not find player to teleport.");
            return;
        }

        CharacterController controller = targetPlayer.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        targetPlayer.position = upstairsTeleportPoint.position;
        targetPlayer.rotation = upstairsTeleportPoint.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = _fadeCanvasGroup.alpha;
        float elapsed = 0f;

        _fadeCanvasGroup.blocksRaycasts = true;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        _fadeCanvasGroup.alpha = targetAlpha;
        _fadeCanvasGroup.blocksRaycasts = targetAlpha > 0f;
    }

    private void CreateFadeCanvas()
    {
        GameObject canvasGO = new GameObject("ElevatorFadeCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject panelGO = new GameObject("FadePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);

        Image image = panelGO.AddComponent<Image>();
        image.color = Color.black;

        RectTransform rect = panelGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        _fadeCanvasGroup = canvasGO.AddComponent<CanvasGroup>();
        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;
    }
}
