using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI interactionText;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ShowHoverText(string text, Vector3 worldPosition)
{
    if (interactionText == null) return;

    interactionText.text = text;
    interactionText.gameObject.SetActive(true);

    Vector3 targetPos = Camera.main.WorldToScreenPoint(worldPosition);
    
    interactionText.transform.position = Vector3.Lerp(interactionText.transform.position, targetPos, 0.2f);
}

    public void HideHoverText()
    {
        interactionText.gameObject.SetActive(false);
    }
}