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

        hoverImage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverImage.enabled = false;
    }
}