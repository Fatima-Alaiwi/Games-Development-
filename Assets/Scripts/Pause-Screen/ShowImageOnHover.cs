using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowImageOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image hoverImage;

    private void Awake()
    {
        if (hoverImage == null)
            hoverImage = GetComponent<Image>();

        if (hoverImage == null)
        {
            Debug.LogWarning("ShowImageOnHover needs an Image reference.", this);
            enabled = false;
            return;
        }

        hoverImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverImage != null)
            hoverImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverImage != null)
            hoverImage.enabled = false;
    }
}
