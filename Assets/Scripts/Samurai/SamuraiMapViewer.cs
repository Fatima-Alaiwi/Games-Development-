using UnityEngine;

public class SamuraiMapViewer : MonoBehaviour
{
    [Header("Map Canvas")]
    public GameObject mapCanvas;

    [Header("Item Name — must match what Farmer adds")]
    public string mapItemName = "SamuraiMap";

    private bool _isOpen = false;
    private int _selectedIndex = 0;

    void Update()
    {
        TrackSelection();

        if (!Input.GetKeyDown(KeyCode.Q)) return;
        if (!IsMapSelected()) return;

        _isOpen = !_isOpen;
        if (_isOpen) OpenMap(); else CloseMap();
    }

    void TrackSelection()
    {
        // Mirror InventorySlotSelector scroll — slots is public so no shared script changes needed
        int totalSlots = (InventorySlotSelector.instance != null)
            ? InventorySlotSelector.instance.slots.Length : 1;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            _selectedIndex--;
            if (_selectedIndex < 0) _selectedIndex = totalSlots - 1;
        }
        else if (scroll < 0f)
        {
            _selectedIndex++;
            if (_selectedIndex >= totalSlots) _selectedIndex = 0;
        }
    }

    bool IsMapSelected()
    {
        if (InventoryManager.instance == null) return false;
        if (_selectedIndex >= InventoryManager.instance.items.Count) return false;
        return InventoryManager.instance.items[_selectedIndex].itemName == mapItemName;
    }

    void OpenMap()
    {
        if (mapCanvas != null) mapCanvas.SetActive(true);
        if (PlayerController.instance != null) PlayerController.instance.canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseMap()
    {
        if (mapCanvas != null) mapCanvas.SetActive(false);
        if (PlayerController.instance != null) PlayerController.instance.canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
