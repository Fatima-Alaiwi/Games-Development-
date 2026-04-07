using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;

    public Sprite defaultSprite; 

    void Start()
    {
        ClearSlot(); // initialize empty look
    }

    public void UpdateSlot(InventoryManager.InventoryItem item)
    {
        icon.sprite = item.icon;
        countText.text = item.count.ToString();
    }

    public void ClearSlot()
    {
        icon.sprite = defaultSprite; 
        countText.text = "";
    }
}