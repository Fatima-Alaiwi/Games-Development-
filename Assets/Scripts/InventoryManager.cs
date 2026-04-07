using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public Sprite icon;
        public int count;
    }

    public List<InventoryItem> items = new List<InventoryItem>();
    public InventorySlot[] slots; // assign in inspector

    public void AddItem(string name, Sprite icon)
    {
        // Check if item already exists
        foreach (var item in items)
        {
            if (item.itemName == name)
            {
                item.count++;
                UpdateUI();
                return;
            }
        }

        // Add new item
        InventoryItem newItem = new InventoryItem();
        newItem.itemName = name;
        newItem.icon = icon;
        newItem.count = 1;

        items.Add(newItem);
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
            {
                slots[i].UpdateSlot(items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}