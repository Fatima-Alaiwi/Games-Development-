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
    public InventorySlot[] slots;

    public bool AddItem(string name, Sprite icon)
    {
        Debug.Log("Trying to add: " + name + " with icon: " + (icon != null ? icon.name : "NULL"));
        foreach (var item in items)
        {
            if (item.itemName == name)
            {
                item.count++;
                UpdateUI();
                return true;
            }
        }

        if (items.Count >= slots.Length)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        InventoryItem newItem = new InventoryItem();
        newItem.itemName = name;
        newItem.icon = icon;
        newItem.count = 1;

        items.Add(newItem);
        UpdateUI();
        return true;
        
    }

    public bool RemoveItem(string name, int amount = 1)
    {
        foreach (var item in items)
        {
            if (item.itemName == name)
            {
                item.count -= amount;
                if (item.count <= 0)
                    items.Remove(item);
                UpdateUI();
                return true;
            }
        }
        return false;
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
                slots[i].UpdateSlot(items[i]);
            else
                slots[i].ClearSlot();
        }
    }

    public bool HasItem(string name)
    {
        foreach (var item in items)
        {
            if (item.itemName == name)
                return true;
        }
        return false;
    }
    public int GetItemCount(string name)
    {
        foreach (var item in items)
        {
            if (item.itemName == name)
            {
                return item.count; // Found the item! Return the quantity Peter has
            }
        }
        return 0; // Item not found, return 0
    }

}