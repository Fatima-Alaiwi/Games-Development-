using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI interactionText;

    private Camera _cam;

    void Awake()
    {
        if (Instance == null) Instance = this;
        _cam = Camera.main;
        if (_cam == null) _cam = FindFirstObjectByType<Camera>();
    }

    public void ShowHoverText(string text, Vector3 worldPosition)
{
    if (interactionText == null) return;
    if (_cam == null) return;

    interactionText.text = text;
    interactionText.gameObject.SetActive(true);

    Vector3 targetPos = _cam.WorldToScreenPoint(worldPosition);
    
    interactionText.transform.position = Vector3.Lerp(interactionText.transform.position, targetPos, 0.2f);
}

    public void HideHoverText()
    {
        interactionText.gameObject.SetActive(false);
    }
}