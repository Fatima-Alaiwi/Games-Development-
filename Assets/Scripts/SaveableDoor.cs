using UnityEngine;

// Attach to any door GameObject. Give it a unique doorId in the Inspector.
// The save system uses this to reopen the door when loading a save.
public class SaveableDoor : MonoBehaviour
{
    [Tooltip("Must be unique within the scene, e.g. 'dun_door_1'.")]
    public string doorId;

    [HideInInspector] public bool isOpened = false;

    public void MarkOpened()
    {
        if (string.IsNullOrEmpty(doorId))
        {
            Debug.LogWarning($"[SaveableDoor] '{gameObject.name}' has no doorId set — won't be tracked.");
            return;
        }
        isOpened = true;
    }

    public void RestoreOpen()
    {
        isOpened = true;

        var door = GetComponent<Door>();
        if (door != null) { door.SnapOpen(); return; }

        var dungeonDoor = GetComponent<DungeonLockedDoor>();
        if (dungeonDoor != null) { dungeonDoor.SnapOpen(); return; }

        var keyDoor = GetComponent<KeyDoor>();
        if (keyDoor != null) { keyDoor.SnapOpen(); return; }

        var questDoor = GetComponent<QuestLockedDoor>();
        if (questDoor != null) { questDoor.SnapOpen(); return; }

        var mansionDoor = GetComponent<HorrorMansionLockedDoor>();
        if (mansionDoor != null) { mansionDoor.SnapOpen(); return; }

        var lockedDoor = GetComponent<LockedDoor>();
        if (lockedDoor != null) { lockedDoor.SnapOpen(); return; }

        Debug.LogWarning($"[SaveableDoor] '{gameObject.name}' — no recognised door script found on this GameObject. Make sure SaveableDoor is on the same object as the door script.");
    }
}
