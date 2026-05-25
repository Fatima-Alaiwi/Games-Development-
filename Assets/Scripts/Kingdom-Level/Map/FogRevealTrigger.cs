using UnityEngine;

/// <summary>
/// Place on an invisible trigger volume at each map location.
/// When Peter walks into it the fog lifts and the pin becomes visible.
///
/// Setup:
///   1. Create an empty GameObject at the location in the scene.
///   2. Add a BoxCollider or SphereCollider, tick "Is Trigger".
///   3. Attach this script.
///   4. Assign the matching MapLocation asset and MapUI reference.
/// </summary>
[RequireComponent(typeof(Collider))]
public class FogRevealTrigger : MonoBehaviour
{
    [Header("References")]
    public MapLocation location;   // the matching MapLocation ScriptableObject
    public MapUI       mapUI;      // drag MapPanel here (or leave null to auto-find)

    [Header("Settings")]
    [Tooltip("Only reveal for objects on this layer (should be the Player layer).")]
    public string playerTag = "Player";

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (mapUI == null)
            mapUI = FindAnyObjectByType<MapUI>(FindObjectsInactive.Include);

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (location == null || mapUI == null) return;

        // Lift fog around this location and show pin
        mapUI.RevealLocationPin(location);

        // Optionally auto-mark as reached (MapUI also does a distance check,
        // but the trigger is more reliable for tight indoor spaces)
        mapUI.MarkLocationReached(location);

        // Disable trigger after first use — no need to fire again
        enabled = false;
    }
}
